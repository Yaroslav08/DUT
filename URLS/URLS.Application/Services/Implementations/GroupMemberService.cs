using AutoMapper;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group.GroupMember;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace URLS.Application.Services.Implementations
{
    public class GroupMemberService: BaseService<UserGroup>, IGroupMemberService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICommonService _commonService;
        public GroupMemberService(URLSDbContext db, IMapper mapper, IIdentityService identityService, ICommonService commonService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
            _commonService = commonService;
        }

        public async Task<Result<bool>> AcceptAllNewGroupMembersAsync(int groupId)
        {
            if (!await CanAcceptNewJoinersAsync(groupId))
                return Result<bool>.Forbiden();

            var allNewGroupMembers = await _db.UserGroups
                .AsNoTracking()
                .Where(s => s.Status == UserGroupStatus.New && s.GroupId == groupId)
                .ToListAsync();

            if (allNewGroupMembers == null || allNewGroupMembers.Count == 0)
                return Result<bool>.Success();

            allNewGroupMembers.ForEach(gm =>
            {
                gm.Status = UserGroupStatus.Member;
                gm.PrepareToUpdate(_identityService);
            });

            _db.UserGroups.UpdateRange(allNewGroupMembers);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<bool>> AcceptNewGroupMemberAsync(int groupId, int groupMemberId)
        {
            if (!await CanAcceptNewJoinersAsync(groupId))
                return Result<bool>.Forbiden();

            var newGroupMember = await _db.UserGroups
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == groupMemberId);

            if (newGroupMember == null)
                return Result<bool>.NotFound(typeof(UserGroup).NotFoundMessage(groupMemberId));

            if (newGroupMember.GroupId != groupId)
                return Result<bool>.Error("Member not from current group");

            if (newGroupMember.Status == UserGroupStatus.Member)
                return Result<bool>.Error("User is already member of group");

            newGroupMember.Status = UserGroupStatus.Member;
            newGroupMember.PrepareToUpdate(_identityService);

            _db.UserGroups.Update(newGroupMember);
            await _db.SaveChangesAsync();

            return Result<bool>.Success();
        }

        public async Task<Result<GroupMemberViewModel>> GetGroupMemberByIdAsync(int groupId, int memberId)
        {
            if (!await _commonService.IsExistAsync<Group>(x => x.Id == groupId))
                return Result<GroupMemberViewModel>.NotFound(typeof(Group).NotFoundMessage(groupId));

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
            if (!await _commonService.IsExistAsync<Group>(x => x.Id == groupId))
                return Result<List<GroupMemberViewModel>>.NotFound(typeof(Group).NotFoundMessage(groupId));

            var userGroupRoles = new List<UserGroupRole>();

            var query = _db.UserGroups
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.GroupId == groupId && x.UserId < afterId)
                .OrderByDescending(x => x.Id)
                .Take(count);
            if (status > 0 && status < 4)
            {
                query = query.Where(x => x.Status == (UserGroupStatus)status);
            }

            var groupMembers = await query.ToListAsync();

            foreach (var groupMember in groupMembers)
            {
                var userGroupRole = userGroupRoles.FirstOrDefault(s => s.Id == groupMember.UserGroupRoleId);
                if (userGroupRole == null)
                {
                    var currentUserGroupRole = await _db.UserGroupRoles.AsNoTracking().FirstOrDefaultAsync(s => s.Id == groupMember.UserGroupRoleId);
                    userGroupRoles.Add(currentUserGroupRole);
                    groupMember.UserGroupRole = currentUserGroupRole;
                }
                else
                {
                    groupMember.UserGroupRole = userGroupRole;
                }
            }

            if (groupMembers == null)
                return Result<List<GroupMemberViewModel>>.Success();

            var groupMembersToView = groupMembers.MapToViews(false);
            return Result<List<GroupMemberViewModel>>.SuccessWithData(groupMembersToView);
        }

        public async Task<Result<GroupMemberViewModel>> UpdateGroupMemberAsync(GroupMemberEditModel model)
        {
            if (!await _commonService.IsExistAsync<Group>(s => s.Id == model.GroupId))
                return Result<GroupMemberViewModel>.NotFound(typeof(Group).NotFoundMessage(model.GroupId));

            if (!await _commonService.IsExistAsync<UserGroupRole>(s => s.Id == model.UserGroupRoleId))
                return Result<GroupMemberViewModel>.NotFound(typeof(UserGroupRole).NotFoundMessage(model.UserGroupRoleId));

            var currentGroupMember = await _db.UserGroups.FindAsync(model.Id);
            if (currentGroupMember == null)
                return Result<GroupMemberViewModel>.NotFound(typeof(UserGroup).NotFoundMessage(model.Id));

            if (currentGroupMember.GroupId != model.GroupId)
                return Result<GroupMemberViewModel>.Forbiden();

            currentGroupMember.Title = model.Title;
            currentGroupMember.Status = model.Status;
            currentGroupMember.UserGroupRoleId = model.UserGroupRoleId;
            currentGroupMember.PrepareToUpdate(_identityService);

            _db.UserGroups.Update(currentGroupMember);
            await _db.SaveChangesAsync();

            return Result<GroupMemberViewModel>.Success();
        }

        private async Task<bool> CanAcceptNewJoinersAsync(int groupId)
        {
            if (_identityService.IsAdministrator())
                return true;

            var currentUserId = _identityService.GetUserId();

            var groupMember = await _db.UserGroups.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == currentUserId && s.GroupId == groupId);
            if (groupMember == null)
                return false;
            return groupMember.IsAdmin;
        }
    }
}