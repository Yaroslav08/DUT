using AutoMapper;
using URLS.Application.Extensions;
using URLS.Application.Options;
using URLS.Application.Services.Interfaces;
using URLS.Application.Validations;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group;
using URLS.Application.ViewModels.Group.GroupMember;
using URLS.Constants;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
namespace URLS.Application.Services.Implementations
{
    public class GroupService : BaseService<Group>, IGroupService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;
        private readonly IPostService _postService;
        private readonly ICommonService _commonService;
        public GroupService(URLSDbContext db, IMapper mapper, IIdentityService identityService, IPostService postService, IUserService userService, ICommonService commonService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
            _postService = postService;
            _userService = userService;
            _commonService = commonService;
        }

        public async Task<Result<GroupViewModel>> CreateGroupAsync(GroupCreateModel model)
        {
            if (await _commonService.IsExistAsync<Group>(s => s.Name == model.Name && s.StartStudy == model.StartStudy))
                return Result<GroupViewModel>.Error("Same group already exist");

            if (!await _commonService.IsExistAsync<Specialty>(s => s.Id == model.SpecialtyId))
                return Result<GroupViewModel>.NotFound("Specialty not found");

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

            return Result<GroupViewModel>.SuccessWithData(groupToView);
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
            var groupsToView = _mapper.Map<List<GroupViewModel>>(groups);
            return Result<List<GroupViewModel>>.SuccessWithData(groupsToView);
        }

        public async Task<Result<GroupViewModel>> GetGroupByIdAsync(int id)
        {
            if (!await IsExistAsync(s => s.Id == id))
                return Result<GroupViewModel>.NotFound($"Group with ID ({id}) not found");
            var groupToView = _mapper.Map<GroupViewModel>(Exists.First());
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
            return Result<List<GroupViewModel>>.SuccessWithData(_mapper.Map<List<GroupViewModel>>(groups));
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

            return Result<List<GroupShortViewModel>>.SuccessWithData(groups);
        }

        public async Task<Result<GroupViewModel>> IncreaseCourseOfGroupAsync(int groupId)
        {
            if (!await IsExistAsync(x => x.Id == groupId))
                return Result<GroupViewModel>.NotFound($"Group with ID ({groupId}) not found");

            var group = Exists.First();

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
            if (!await IsExistAsync(s => s.Id == model.GroupId.Value))
                return Result<GroupMemberViewModel>.NotFound(typeof(Group).NotFoundMessage(model.GroupId.Value));

            var currentUserGroup = await _db.UserGroups
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.GroupId == model.GroupId.Value && s.IsAdmin);

            if (currentUserGroup == null)
                return Result<GroupMemberViewModel>.NotFound(typeof(UserGroup).NotFoundMessage(model.GroupId.Value));

            if (currentUserGroup.UserId == model.UserId)
                return Result<GroupMemberViewModel>.Success();

            if (!await _userService.IsExistAsync(s => s.Id == model.UserId))
                return Result<GroupMemberViewModel>.NotFound(typeof(User).NotFoundMessage(model.UserId));

            currentUserGroup.UserId = model.UserId;
            currentUserGroup.Title = model.Title;
            currentUserGroup.PrepareToUpdate(_identityService);
            _db.UserGroups.Update(currentUserGroup);
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