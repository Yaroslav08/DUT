using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Export;
using URLS.Application.ViewModels.Group.GroupMember;
using URLS.Application.ViewModels.User;
using URLS.Constants.APIResponse;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly URLSDbContext _db;
        private readonly IIdentityService _identityService;
        private readonly ICommonService _commonService;
        private readonly ISessionService _sessionService;
        private readonly ISessionManager _sessionManager;
        public GroupMemberService(URLSDbContext db, IIdentityService identityService, ICommonService commonService, ISessionService sessionService, ISessionManager sessionManager)
        {
            _db = db;
            _identityService = identityService;
            _commonService = commonService;
            _sessionService = sessionService;
            _sessionManager = sessionManager;
        }

        public async Task<Result<bool>> AcceptAllNewGroupMembersAsync(int groupId)
        {
            if (!await CanAcceptOrRejectNewJoinersAsync(groupId))
                return Result<bool>.Forbiden();

            var allNewGroupMembers = await _db.UserGroups
                .AsNoTracking()
                .Where(s => s.Status == UserGroupStatus.New && s.GroupId == groupId)
                .Include(s => s.User)
                .ToListAsync();

            if (allNewGroupMembers == null || allNewGroupMembers.Count == 0)
                return Result<bool>.Success();

            allNewGroupMembers.ForEach(gm =>
            {
                gm.Status = UserGroupStatus.Member;
                gm.PrepareToUpdate(_identityService);
                gm.User.IsActivateAccount = true;
            });

            _db.UserGroups.UpdateRange(allNewGroupMembers);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<bool>> AcceptNewGroupMemberAsync(int groupId, int groupMemberId, UserEditModel userModel)
        {
            if (!await CanAcceptOrRejectNewJoinersAsync(groupId))
                return Result<bool>.Forbiden();

            var newGroupMember = await _db.UserGroups
                .AsNoTracking()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == groupMemberId);

            if (newGroupMember == null)
                return Result<bool>.NotFound(typeof(UserGroup).NotFoundMessage(groupMemberId));

            if (newGroupMember.GroupId != groupId)
                return Result<bool>.Error("Member not from current group");

            if (newGroupMember.Status == UserGroupStatus.Member)
                return Result<bool>.Error("User is already member of group");

            var newUser = newGroupMember.User;
            if (userModel != null)
            {
                newUser.FirstName = userModel.FirstName;
                newUser.LastName = userModel.LastName;
                newUser.MiddleName = userModel.MiddleName;
            }
            newUser.IsActivateAccount = true;
            newUser.PrepareToUpdate(_identityService);

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

        public async Task<Result<List<GroupMemberViewModel>>> GetGroupMembersAsync(int groupId, int offset = 0, int count = 20, int status = 0)
        {
            if (!await _commonService.IsExistAsync<Group>(x => x.Id == groupId))
                return Result<List<GroupMemberViewModel>>.NotFound(typeof(Group).NotFoundMessage(groupId));

            var userGroupRoles = new List<UserGroupRole>();

            var query = _db.UserGroups
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.GroupId == groupId)
                .OrderByDescending(x => x.Id)
                .Skip(offset).Take(count);

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

            var totalCount = await _db.UserGroups.CountAsync(x => x.GroupId == groupId);

            var groupMembersToView = groupMembers.MapToViews(false);
            return Result<List<GroupMemberViewModel>>.SuccessList(groupMembersToView, Meta.FromMeta(totalCount, offset, count));
        }

        public async Task<Result<bool>> RejectNewGroupMemberAsync(int groupId, int groupMemberId)
        {
            if (!await CanAcceptOrRejectNewJoinersAsync(groupId))
                return Result<bool>.Forbiden();

            var groupMember = await _db.UserGroups.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == groupMemberId && s.Status == UserGroupStatus.New);
            if (groupMember == null)
                return Result<bool>.NotFound(typeof(UserGroup).NotFoundMessage(groupMemberId));

            _db.Users.Remove(groupMember.User);
            await _db.SaveChangesAsync();

            return Result<bool>.SuccessWithData(true);
        }

        public async Task<Result<GroupMemberViewModel>> UpdateGroupMemberAsync(GroupMemberEditModel model)
        {
            if (!await _commonService.IsExistAsync<Group>(s => s.Id == model.GroupId))
                return Result<GroupMemberViewModel>.NotFound(typeof(Group).NotFoundMessage(model.GroupId));

            if (!await _commonService.IsExistAsync<UserGroupRole>(s => s.Id == model.UserGroupRoleId))
                return Result<GroupMemberViewModel>.NotFound(typeof(UserGroupRole).NotFoundMessage(model.UserGroupRoleId));

            var currentGroupMember = await _db.UserGroups.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == model.Id);
            if (currentGroupMember == null)
                return Result<GroupMemberViewModel>.NotFound(typeof(UserGroup).NotFoundMessage(model.Id));

            if (currentGroupMember.GroupId != model.GroupId)
            {
                currentGroupMember.Status = UserGroupStatus.Gona;
                currentGroupMember.PrepareToUpdate(_identityService);

                var newGroupMember = new UserGroup
                {
                    Title = model.Title,
                    Status = UserGroupStatus.Member,
                    UserId = currentGroupMember.UserId,
                    GroupId = currentGroupMember.GroupId,
                    UserGroupRoleId = model.UserGroupRoleId,
                };

                newGroupMember.PrepareToCreate(_identityService);
                await _db.UserGroups.AddAsync(newGroupMember);
            }
            else
            {
                currentGroupMember.Title = model.Title;
                currentGroupMember.Status = model.Status;
                currentGroupMember.UserGroupRoleId = model.UserGroupRoleId;
                currentGroupMember.PrepareToUpdate(_identityService);
            }

            _db.UserGroups.Update(currentGroupMember);

            if (model.User != null)
            {
                var user = currentGroupMember.User;
                user.FirstName = model.User.FirstName;
                user.MiddleName = model.User.MiddleName;
                user.LastName = model.User.LastName;
                user.PrepareToUpdate(_identityService);
                _db.Users.Update(user);
                await _sessionService.CloseAllSessionsAsync(currentGroupMember.User.Id);
            }

            await _db.SaveChangesAsync();

            return Result<GroupMemberViewModel>.Success();
        }

        private async Task<bool> CanAcceptOrRejectNewJoinersAsync(int groupId)
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