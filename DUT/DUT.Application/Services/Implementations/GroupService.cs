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
using System.Collections.Generic;

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

            var groupMembersToView = groupMembers.Select(x => new GroupMemberViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                IsAdmin = x.IsAdmin,
                Status = x.Status,
                Title = x.Title,
                User = new ViewModels.User.UserViewModel
                {
                    Id = x.User.Id,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    ContactEmail = x.User.ContactEmail,
                    ContactPhone = x.User.ContactPhone,
                    FullName = $"{x.User.FirstName} {x.User.LastName}",
                    Image = x.User.Image,
                    MiddleName = x.User.MiddleName,
                    UserName = x.User.UserName,
                    JoinAt = x.User.JoinAt
                },
                UserGroupRole = new UserGroupRoleViewModel
                {
                    Id = x.UserGroupRole.Id,
                    CreatedAt = x.UserGroupRole.CreatedAt,
                    Name = x.UserGroupRole.Name,
                    NameEng = x.UserGroupRole.NameEng,
                    Color = x.UserGroupRole.Color,
                    Description = x.UserGroupRole.Description,
                    DescriptionEng = x.UserGroupRole.DescriptionEng,
                    Permissions = x.UserGroupRole.Permissions
                }
            }).ToList();
            return Result<List<GroupMemberViewModel>>.SuccessWithData(groupMembersToView);
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
