using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Group;
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
    }
}
