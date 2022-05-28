using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Options;
using URLS.Application.Services.Interfaces;
using URLS.Application.Validations;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group;
using URLS.Application.ViewModels.Group.GroupMember;
using URLS.Constants;
using URLS.Constants.APIResponse;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICommonService _commonService;
        public GroupService(URLSDbContext db, IMapper mapper, IIdentityService identityService, ICommonService commonService)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
            _commonService = commonService;
        }

        public async Task<Result<GroupViewModel>> CreateGroupAsync(GroupCreateModel model)
        {
            if (await _commonService.IsExistAsync<Group>(s => s.Name == model.Name && s.StartStudy == model.StartStudy))
                return Result<GroupViewModel>.Error("Same group already exist");

            if (!await _commonService.IsExistAsync<Specialty>(s => s.Id == model.SpecialtyId))
                return Result<GroupViewModel>.NotFound(typeof(Specialty).NotFoundMessage(model.SpecialtyId));

            if (!model.TryValidateGroupName(out var error))
            {
                return Result<GroupViewModel>.Error(error);
            }

            var newGroup = new Group
            {
                Name = model.Name,
                Course = model.Course,
                StartStudy = model.StartStudy,
                EndStudy = model.EndStudy,
                SpecialtyId = model.SpecialtyId,
                Image = model.Image ?? Defaults.GroupImage
            };

            if (model.TryGetIndexForNumber(out var index))
            {
                newGroup.IndexNumber = index;
            }

            newGroup.PrepareToCreate(_identityService);

            await _db.Groups.AddAsync(newGroup);
            await _db.SaveChangesAsync();

            var newInvite = new GroupInvite
            {
                ActiveFrom = Defaults.GroupInviteActiveFrom,
                ActiveTo = Defaults.GroupInviteActiveTo,
                CodeJoin = Generator.CreateGroupInviteCode(),
                GroupId = newGroup.Id,
                IsActive = true,
                Name = "Головне запрошення"
            };
            newInvite.PrepareToCreate(_identityService);

            await _db.GroupInvites.AddAsync(newInvite);
            await _db.SaveChangesAsync();

            if (model.ClassTeacherId.HasValue)
            {
                var userGroupRole = await _db.UserGroupRoles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UniqId == UserGroupRoles.UniqIds.ClassTeacher);

                var groupMember = new UserGroup
                {
                    GroupId = newGroup.Id,
                    UserId = model.ClassTeacherId.Value,
                    IsAdmin = true,
                    Status = UserGroupStatus.Member,
                    Title = UserGroupRoles.Names.ClassTeacher,
                    UserGroupRoleId = userGroupRole.Id,
                };
                groupMember.PrepareToCreate(_identityService);
                await _db.UserGroups.AddAsync(groupMember);
                await _db.SaveChangesAsync();
            }

            var groupFromDb = await _db.Groups.AsNoTracking().FirstOrDefaultAsync(x => x.Id == newGroup.Id);

            var groupToView = _mapper.Map<GroupViewModel>(groupFromDb);

            groupToView.CountOfStudents = await _db.UserGroups.CountAsync(x => x.GroupId == newGroup.Id && x.Status == Domain.Models.UserGroupStatus.Member);

            groupToView.GroupInvites = await _db.GroupInvites.AsNoTracking().Where(x => x.GroupId == newGroup.Id).Select(s => new GroupInviteViewModel
            {
                Id = s.Id,
                CreatedAt = s.CreatedAt,
                Name = s.Name,
                CodeJoin = s.CodeJoin,
                ActiveFrom = s.ActiveFrom,
                ActiveTo = s.ActiveTo,
                IsActive = s.IsActive
            }).ToListAsync();

            return Result<GroupViewModel>.Created(groupToView);
        }

        public async Task<Result<List<GroupViewModel>>> GetAllGroupsAsync(int offset = 0, int limit = 20)
        {
            var groups = await _db.Groups
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Skip(offset).Take(limit)
                .ToListAsync();
            if (groups == null || groups.Count == 0)
                return Result<List<GroupViewModel>>.Success();

            var totalCount = await _db.Groups.CountAsync();
            var groupsToView = _mapper.Map<List<GroupViewModel>>(groups);

            return Result<List<GroupViewModel>>.SuccessList(groupsToView, Meta.FromMeta(totalCount, offset, limit));
        }

        public async Task<Result<GroupViewModel>> GetGroupByIdAsync(int id)
        {
            var query = await _commonService.IsExistWithResultsAsync<Group>(s => s.Id == id);

            if (!query.IsExist)
                return Result<GroupViewModel>.NotFound(typeof(Group).NotFoundMessage(id));
            var groupToView = _mapper.Map<GroupViewModel>(query.Results.First());
            groupToView.CountOfStudents = await _db.UserGroups.CountAsync(x => x.GroupId == id && x.Status == Domain.Models.UserGroupStatus.Member);
            return Result<GroupViewModel>.SuccessWithData(groupToView);
        }

        public async Task<Result<List<GroupViewModel>>> GetGroupsBySpecialtyIdAsync(int specialtyId)
        {
            var groups = await _db.Groups
                .AsNoTracking()
                .Where(s => s.SpecialtyId == specialtyId)
                .OrderBy(s => s.Name).ThenBy(s => s.Course)
                .ToListAsync();

            var totalCount = await _commonService.CountAsync<Group>(s => s.SpecialtyId == specialtyId);

            var groupsViewModels = _mapper.Map<List<GroupViewModel>>(groups);

            return Result<List<GroupViewModel>>.SuccessList(groupsViewModels, Meta.FromMeta(totalCount, 0, 0));
        }

        public async Task<Result<List<GroupShortViewModel>>> GetUserGroupsAsync(int userId)
        {
            var userGroups = await _db.UserGroups
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .Include(s => s.Group)
                .Select(s => s.Group)
                .OrderByDescending(s => s.Id)
                .ToListAsync();
            var groups = _mapper.Map<List<GroupShortViewModel>>(userGroups);

            var totalCount = await _commonService.CountAsync<UserGroup>(s => s.UserId == userId);

            return Result<List<GroupShortViewModel>>.SuccessList(groups, Meta.FromMeta(totalCount, 0, 0));
        }

        public async Task<Result<GroupViewModel>> IncreaseCourseOfGroupAsync(int groupId)
        {
            var query = await _commonService.IsExistWithResultsAsync<Group>(x => x.Id == groupId);

            if (!query.IsExist)
                return Result<GroupViewModel>.NotFound(typeof(Group).NotFoundMessage(groupId));

            var group = query.Results.First();

            if (group.Course >= 6)
                return Result<GroupViewModel>.Error($"Group course can`t be more then {group.Course}");

            group.IncreaseCourse();

            group.PrepareToUpdate(_identityService);
            _db.Groups.Update(group);
            await _db.SaveChangesAsync();

            return Result<GroupViewModel>.SuccessWithData(_mapper.Map<GroupViewModel>(group));
        }

        public async Task<Result<List<GroupViewModel>>> SearchGroupsAsync(SearchGroupOptions options)
        {
            options.PrepareOptions();

            var query = _db.Groups.AsNoTracking();

            if (!string.IsNullOrEmpty(options.Name))
                query = query.Where(s => s.Name.Contains(options.Name));

            if (options.Course != null)
                query = query.Where(s => s.Course == options.Course);

            if (options.SpecialtyId != null)
                query = query.Where(s => s.SpecialtyId == options.SpecialtyId);

            if (options.From != null)
                query = query.Where(s => s.StartStudy == options.From);

            if (options.To != null)
                query = query.Where(s => s.EndStudy == options.To);

            query = query.Skip(options.Offset).Take(options.Count);

            query = query.OrderBy(s => s.Name);

            var groups = await query.ToListAsync();

            var groupsToView = _mapper.Map<List<GroupViewModel>>(groups);

            return Result<List<GroupViewModel>>.SuccessWithData(groupsToView);
        }

        public async Task<Result<GroupMemberViewModel>> UpdateClassTeacherGroupAsync(GroupClassTeacherEditModel model)
        {
            if (!_identityService.IsAdministrator())
                return Result<GroupMemberViewModel>.Forbiden();

            var query = await _commonService.IsExistWithResultsAsync<Group>(s => s.Id == model.GroupId.Value);

            if (!query.IsExist)
                return Result<GroupMemberViewModel>.NotFound(typeof(Group).NotFoundMessage(model.GroupId.Value));

            if (!await _commonService.IsExistAsync<User>(s => s.Id == model.UserId))
                return Result<GroupMemberViewModel>.NotFound(typeof(User).NotFoundMessage(model.UserId));

            var currentUserGroup = await _db.UserGroups
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.GroupId == model.GroupId.Value && s.IsAdmin);

            if (currentUserGroup == null)
            {

                var exist = await _commonService.IsExistWithResultsAsync<UserGroupRole>(s => s.UniqId == UserGroupRoles.UniqIds.ClassTeacher);
                if (!exist.IsExist)
                    return Result<GroupMemberViewModel>.NotFound(typeof(UserGroupRole).NotFoundMessage(UserGroupRoles.UniqIds.ClassTeacher));

                currentUserGroup = new UserGroup
                {
                    GroupId = model.GroupId.Value,
                    IsAdmin = true,
                    Status = UserGroupStatus.Member,
                    Title = model.Title,
                    UserGroupRoleId = exist.Results.First().Id,
                    UserId = model.UserId
                };

                currentUserGroup.PrepareToUpdate(_identityService);

            }
            else
            {
                if (currentUserGroup.UserId == model.UserId)
                    return Result<GroupMemberViewModel>.Success();
                currentUserGroup.UserId = model.UserId;
                currentUserGroup.Title = model.Title;
                currentUserGroup.PrepareToUpdate(_identityService);
                _db.UserGroups.Update(currentUserGroup);
            }

            await _db.SaveChangesAsync();

            var groupMember = await _db.UserGroups
                .AsNoTracking()
                .Include(s => s.User)
                .Include(s => s.UserGroupRole)
                .FirstOrDefaultAsync(s => s.Id == currentUserGroup.Id);

            var updatedGroupMember = _mapper.Map<GroupMemberViewModel>(groupMember);

            return Result<GroupMemberViewModel>.SuccessWithData(updatedGroupMember);
        }
    }
}