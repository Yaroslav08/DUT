using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Group;
using DUT.Application.ViewModels.Group.GroupMember;
using DUT.Constants;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class GroupService : BaseService<Group>, IGroupService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public GroupService(DUTDbContext db, IMapper mapper, IIdentityService identityService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<GroupViewModel>> CreateGroupAsync(GroupCreateModel model)
        {
            if (await IsExistAsync(s => s.Name == model.Name && s.StartStudy == model.StartStudy))
                return Result<GroupViewModel>.Error("Same group already exist");

            if (!await _db.Specialties.AsNoTracking().AnyAsync(s => s.Id == model.SpecialtyId))
                return Result<GroupViewModel>.Error("Specialty not found");

            var newGroup = new Group
            {
                Name = model.Name,
                Course = model.Course,
                StartStudy = model.StartStudy,
                SpecialtyId = model.SpecialtyId,
                Image = model.Image ?? Defaults.GroupImage
            };
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


            var userGroupRole = await _db.UserGroupRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UniqId == UserGroupRoles.UniqIds.ClassTeacher);


            var groupMember = new UserGroup
            {
                GroupId = newGroup.Id,
                UserId = _identityService.GetUserId(),
                IsAdmin = true,
                Status = UserGroupStatus.Member,
                Title = UserGroupRoles.Names.ClassTeacher,
                UserGroupRoleId = userGroupRole.Id,
            };
            groupMember.PrepareToCreate(_identityService);
            await _db.UserGroups.AddAsync(groupMember);
            await _db.SaveChangesAsync();

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

        public async Task<Result<GroupInviteViewModel>> CreateGroupInviteAsync(GroupInviteCreateModel model)
        {
            if (!await IsExistAsync(s => s.Id == model.GroupId))
                return Result<GroupInviteViewModel>.NotFound("Group not found");

            if (await _db.GroupInvites.AsNoTracking().CountAsync(s => s.GroupId == model.GroupId) >= 5)
                return Result<GroupInviteViewModel>.Error("One group must be have max 5 invites");

            var newGroupInvite = new GroupInvite
            {
                ActiveFrom = model.ActiveFrom,
                ActiveTo = model.ActiveTo,
                Name = model.Name,
                IsActive = model.IsActive,
                CodeJoin = Generator.CreateGroupInviteCode(),
                GroupId = model.GroupId.Value,
            };
            newGroupInvite.PrepareToCreate(_identityService);
            await _db.GroupInvites.AddAsync(newGroupInvite);
            await _db.SaveChangesAsync();
            return Result<GroupInviteViewModel>.SuccessWithData(_mapper.Map<GroupInviteViewModel>(newGroupInvite));
        }

        public async Task<Result<List<GroupViewModel>>> GetAllGroupsAsync(int count, int afterId)
        {
            var groups = await _db.Groups
                .AsNoTracking()
                .Where(s => s.Id < afterId)
                .OrderByDescending(x => x.Id)
                .Take(count)
                .ToListAsync();
            if (groups == null || groups.Count == 0)
                return Result<List<GroupViewModel>>.Success();
            var groupsToView = _mapper.Map<List<GroupViewModel>>(groups);
            return Result<List<GroupViewModel>>.SuccessWithData(groupsToView);
        }

        public async Task<Result<GroupViewModel>> GetGroupByIdAsync(int id)
        {
            var group = await _db.Groups.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (group == null)
                return Result<GroupViewModel>.NotFound($"Group with ID ({id}) not found");
            var groupToView = _mapper.Map<GroupViewModel>(group);
            groupToView.CountOfStudents = await _db.UserGroups.CountAsync(x => x.GroupId == id && x.Status == Domain.Models.UserGroupStatus.Member);
            return Result<GroupViewModel>.SuccessWithData(groupToView);
        }

        public async Task<Result<List<GroupInviteViewModel>>> GetGroupInvitesByGroupIdAsync(int groupId)
        {
            if (!await IsExistAsync(s => s.Id == groupId))
                return Result<List<GroupInviteViewModel>>.NotFound("Group not found");
            var groupInvitesFromDb = await _db.GroupInvites
                .AsNoTracking()
                .Where(x => x.GroupId == groupId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var groupInvitesToViews = _mapper.Map<List<GroupInviteViewModel>>(groupInvitesFromDb);

            return Result<List<GroupInviteViewModel>>.SuccessWithData(groupInvitesToViews);
        }

        public async Task<Result<GroupMemberViewModel>> GetGroupMemberByIdAsync(int groupId, int memberId)
        {
            if (!await IsExistAsync(x => x.Id == groupId))
                return Result<GroupMemberViewModel>.NotFound($"Group with ID ({groupId}) not found");

            var groupMember = await _db.UserGroups
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.UserGroupRole)
                .FirstOrDefaultAsync(x => x.Id == memberId);

            if (groupMember == null)
                return Result<GroupMemberViewModel>.NotFound($"Member with ID {memberId} not found");

            if (groupMember.GroupId != groupId)
                return Result<GroupMemberViewModel>.Error("Group don't have this member");

            var groupMemberToView = groupMember.MapToView();
            return Result<GroupMemberViewModel>.SuccessWithData(groupMemberToView);
        }

        public async Task<Result<List<GroupMemberViewModel>>> GetGroupMembersAsync(int groupId, int afterId = int.MaxValue, int count = 20, int status = 0)
        {
            if (!await IsExistAsync(x => x.Id == groupId))
                return Result<List<GroupMemberViewModel>>.NotFound($"Group with ID ({groupId}) not found");

            var query = _db.UserGroups
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.UserGroupRole)
                .Where(x => x.GroupId == groupId && x.Id < afterId)
                .OrderByDescending(x => x.Id)
                .Take(count);
            if (status > 0 && status < 4)
            {
                query = query.Where(x => x.Status == (UserGroupStatus)status);
            }

            var groupMembers = await query.ToListAsync();

            if (groupMembers == null)
                return Result<List<GroupMemberViewModel>>.Success();

            var groupMembersToView = groupMembers.MapToViews(false);
            return Result<List<GroupMemberViewModel>>.SuccessWithData(groupMembersToView);
        }

        public async Task<Result<bool>> RemoveGroupInviteAsync(int groupId, Guid groupInviteId)
        {
            var groupInvite = await _db.GroupInvites.AsNoTracking().FirstOrDefaultAsync(x => x.Id == groupInviteId);
            if (groupInvite == null)
                return Result<bool>.NotFound("Group invite not found");

            if (groupInvite.GroupId != groupId)
                return Result<bool>.Error("Incorrect groupId");

            _db.GroupInvites.Remove(groupInvite);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<List<GroupViewModel>>> SearchGroupsAsync(string name)
        {
            var groups = await _db.Groups
                .AsNoTracking()
                .Where(s => s.Name.Contains(name))
                .OrderByDescending(x => x.Id)
                .ToListAsync();
            if (groups == null || groups.Count == 0)
                return Result<List<GroupViewModel>>.Success();
            var groupsToView = _mapper.Map<List<GroupViewModel>>(groups);
            return Result<List<GroupViewModel>>.SuccessWithData(groupsToView);
        }

        public async Task<Result<GroupInviteViewModel>> UpdateGroupInviteAsync(GroupInviteEditModel model)
        {
            var groupInviteFromDb = await _db.GroupInvites.FindAsync(model.Id);
            if (groupInviteFromDb == null)
                return Result<GroupInviteViewModel>.NotFound("Group invite not found");

            groupInviteFromDb.Name = model.Name;
            groupInviteFromDb.ActiveFrom = model.ActiveFrom;
            groupInviteFromDb.ActiveTo = model.ActiveTo;
            groupInviteFromDb.IsActive = model.IsActive;
            groupInviteFromDb.PrepareToUpdate(_identityService);

            _db.GroupInvites.Update(groupInviteFromDb);
            await _db.SaveChangesAsync();

            return Result<GroupInviteViewModel>.SuccessWithData(_mapper.Map<GroupInviteViewModel>(groupInviteFromDb));
        }

        public async Task<Result<GroupMemberViewModel>> UpdateGroupMemberAsync(GroupMemberEditModel model)
        {
            if (!await IsExistAsync(s => s.Id == model.GroupId))
                return Result<GroupMemberViewModel>.NotFound($"Group with ID ({model.GroupId}) not found");

            if (!await _db.UserGroupRoles.AsNoTracking().AnyAsync(s => s.Id == model.UserGroupRoleId))
                return Result<GroupMemberViewModel>.NotFound($"Role with ID ({model.UserGroupRoleId}) not found");

            var currentGroupMember = await _db.UserGroups.FindAsync(model.Id);
            if (currentGroupMember == null)
                return Result<GroupMemberViewModel>.NotFound($"Member not found");

            if (currentGroupMember.GroupId != model.GroupId)
                return Result<GroupMemberViewModel>.Error("Incorrect groupId");

            currentGroupMember.Title = model.Title;
            currentGroupMember.Status = model.Status;
            currentGroupMember.UserGroupRoleId = model.UserGroupRoleId;
            currentGroupMember.PrepareToUpdate(_identityService);

            _db.UserGroups.Update(currentGroupMember);
            await _db.SaveChangesAsync();

            return Result<GroupMemberViewModel>.Success();
        }
    }
}
