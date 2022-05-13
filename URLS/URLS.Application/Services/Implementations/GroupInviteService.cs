using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group;
using URLS.Constants;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class GroupInviteService : IGroupInviteService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly IPermissionGroupInviteService _permissionGroupInviteService;
        public GroupInviteService(URLSDbContext db, IMapper mapper, IIdentityService identityService, IPermissionGroupInviteService permissionGroupInviteService)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
            _permissionGroupInviteService = permissionGroupInviteService;
        }

        public async Task<Result<GroupInviteViewModel>> CreateGroupInviteAsync(GroupInviteCreateModel model)
        {
            if(!await _permissionGroupInviteService.CanCreateInviteAsync(model.GroupId.Value))
                return Result<GroupInviteViewModel>.Forbiden();

            if (!await _db.Groups.AnyAsync(s => s.Id == model.GroupId))
                return Result<GroupInviteViewModel>.NotFound(typeof(Group).NotFoundMessage(model.GroupId));

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
            if (!await _permissionGroupInviteService.CanViewInviteAsync(groupId))
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
            if (!await _permissionGroupInviteService.CanRemoveInviteAsync(groupId))
                return Result<bool>.Forbiden();

            var groupInvite = await _db.GroupInvites.AsNoTracking().FirstOrDefaultAsync(x => x.Id == groupInviteId);
            if (groupInvite == null)
                return Result<bool>.NotFound(typeof(Group).NotFoundMessage(groupId));

            if (groupInvite.GroupId != groupId)
                return Result<bool>.Error("Incorrect groupId");

            _db.GroupInvites.Remove(groupInvite);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<GroupInviteViewModel>> UpdateGroupInviteAsync(GroupInviteEditModel model)
        {
            if(!await _permissionGroupInviteService.CanUpdateInviteAsync(model.GroupId.Value))
                return Result<GroupInviteViewModel>.Forbiden();

            var groupInviteFromDb = await _db.GroupInvites.FindAsync(model.Id);
            if (groupInviteFromDb == null)
                return Result<GroupInviteViewModel>.NotFound(typeof(Group).NotFoundMessage(model.GroupId));

            groupInviteFromDb.Name = model.Name;
            groupInviteFromDb.ActiveFrom = model.ActiveFrom;
            groupInviteFromDb.ActiveTo = model.ActiveTo;
            groupInviteFromDb.IsActive = model.IsActive;
            groupInviteFromDb.PrepareToUpdate(_identityService);

            _db.GroupInvites.Update(groupInviteFromDb);
            await _db.SaveChangesAsync();

            return Result<GroupInviteViewModel>.SuccessWithData(_mapper.Map<GroupInviteViewModel>(groupInviteFromDb));
        }
    }
}