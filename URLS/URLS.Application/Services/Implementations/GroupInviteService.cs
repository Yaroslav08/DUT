using AutoMapper;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group;
using URLS.Constants;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace URLS.Application.Services.Implementations
{
    public class GroupInviteService : BaseService<GroupInvite>, IGroupInviteService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public GroupInviteService(URLSDbContext db, IMapper mapper, IIdentityService identityService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<GroupInviteViewModel>> CreateGroupInviteAsync(GroupInviteCreateModel model)
        {
            if(!await CanCreateInviteAsync(model.GroupId.Value))
                return Result<GroupInviteViewModel>.Forbiden();

            if (!await _db.Groups.AnyAsync(s => s.Id == model.GroupId))
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

        public async Task<Result<List<GroupInviteViewModel>>> GetGroupInvitesByGroupIdAsync(int groupId)
        {
            if (!await CanViewInviteAsync(groupId))
                return Result<List<GroupInviteViewModel>>.Forbiden();

            var groupInvitesFromDb = await _db.GroupInvites
                .AsNoTracking()
                .Where(x => x.GroupId == groupId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var groupInvitesToViews = _mapper.Map<List<GroupInviteViewModel>>(groupInvitesFromDb);

            return Result<List<GroupInviteViewModel>>.SuccessWithData(groupInvitesToViews);
        }

        public async Task<Result<bool>> RemoveGroupInviteAsync(int groupId, Guid groupInviteId)
        {
            if (!await CanRemoveInviteAsync(groupId))
                return Result<bool>.Forbiden();

            var groupInvite = await _db.GroupInvites.AsNoTracking().FirstOrDefaultAsync(x => x.Id == groupInviteId);
            if (groupInvite == null)
                return Result<bool>.NotFound("Group invite not found");

            if (groupInvite.GroupId != groupId)
                return Result<bool>.Error("Incorrect groupId");

            _db.GroupInvites.Remove(groupInvite);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<GroupInviteViewModel>> UpdateGroupInviteAsync(GroupInviteEditModel model)
        {
            if(!await CanUpdateInviteAsync(model.GroupId.Value))
                return Result<GroupInviteViewModel>.Forbiden();

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


        #region Private

        private async Task<bool> CanCreateInviteAsync(int groupId)
        {
            if (_identityService.IsAdministrator())
                return true;
            var member = await _db.UserGroups.AsNoTracking().Include(s => s.UserGroupRole).FirstOrDefaultAsync(s => s.GroupId == groupId && s.UserId == _identityService.GetUserId() && s.Status == UserGroupStatus.Member);
            if (member == null)
                return false;
            if (!member.UserGroupRole.Permissions.CanCreateInviteCode)
                return false;
            return true;
        }

        private async Task<bool> CanViewInviteAsync(int groupId)
        {
            if (_identityService.IsAdministrator())
                return true;
            var member = await _db.UserGroups.AsNoTracking().Include(s => s.UserGroupRole).FirstOrDefaultAsync(s => s.GroupId == groupId && s.UserId == _identityService.GetUserId() && s.Status == UserGroupStatus.Member);
            if (member == null)
                return false;
            if (!member.UserGroupRole.Permissions.CanViewInviteCodes)
                return false;
            return true;
        }

        private async Task<bool> CanRemoveInviteAsync(int groupId)
        {
            if (_identityService.IsAdministrator())
                return true;
            var member = await _db.UserGroups.AsNoTracking().Include(s => s.UserGroupRole).FirstOrDefaultAsync(s => s.GroupId == groupId && s.UserId == _identityService.GetUserId() && s.Status == UserGroupStatus.Member);
            if (member == null)
                return false;
            if (!member.UserGroupRole.Permissions.CanRemoveInviteCode)
                return false;
            return true;
        }

        private async Task<bool> CanUpdateInviteAsync(int groupId)
        {
            if (_identityService.IsAdministrator())
                return true;
            var member = await _db.UserGroups.AsNoTracking().Include(s => s.UserGroupRole).FirstOrDefaultAsync(s => s.GroupId == groupId && s.UserId == _identityService.GetUserId() && s.Status == UserGroupStatus.Member);
            if (member == null)
                return false;
            if (!member.UserGroupRole.Permissions.CanUpdateInviteCode)
                return false;
            return true;
        }

        #endregion
    }
}