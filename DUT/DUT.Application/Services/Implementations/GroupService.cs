using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Options;
using DUT.Application.Services.Interfaces;
using DUT.Application.Validations;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Group;
using DUT.Application.ViewModels.Group.GroupMember;
using DUT.Application.ViewModels.Post;
using DUT.Application.ViewModels.Post.Comment;
using DUT.Constants;
using DUT.Constants.Extensions;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
namespace DUT.Application.Services.Implementations
{
    public class GroupService : BaseService<Group>, IGroupService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        public GroupService(DUTDbContext db, IMapper mapper, IIdentityService identityService, IPostService postService, ICommentService commentService, IUserService userService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
            _postService = postService;
            _commentService = commentService;
            _userService = userService;
        }

        public async Task<Result<bool>> AcceptAllNewGroupMembersAsync(int groupId)
        {
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

        public async Task<Result<CommentViewModel>> CreateCommentAsync(CommentCreateModel model)
        {
            if (!await IsExistAsync(x => x.Id == model.GroupId))
                return Result<CommentViewModel>.NotFound($"Group with ID ({model.GroupId}) not found");

            var group = Exists.First();

            if (!await _postService.IsExistAsync(s => s.Id == model.PostId))
                return Result<CommentViewModel>.NotFound($"Post with ID ({model.PostId}) not found");

            var post = _postService.Exists.First();

            if (post.GroupId != group.Id)
                return Result<CommentViewModel>.NotFound($"This post isn't in this group");

            return await _commentService.CreateCommentAsync(model);
        }

        public async Task<Result<GroupViewModel>> CreateGroupAsync(GroupCreateModel model)
        {
            if (await IsExistAsync(s => s.Name == model.Name && s.StartStudy == model.StartStudy))
                return Result<GroupViewModel>.Error("Same group already exist");

            if (!await _db.Specialties.AsNoTracking().AnyAsync(s => s.Id == model.SpecialtyId))
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

        public async Task<Result<PostViewModel>> CreateGroupPostAsync(PostCreateModel model)
        {
            if (!await IsExistAsync(x => x.Id == model.GroupId))
                return Result<PostViewModel>.NotFound($"Group with ID ({model.GroupId}) not found");
            return await _postService.CreatePostAsync(model);
        }

        public async Task<Result<List<UserGroupRoleViewModel>>> GetAllGroupRolesAsync()
        {
            return Result<List<UserGroupRoleViewModel>>.SuccessWithData(
                _mapper.Map<List<UserGroupRoleViewModel>>(await _db.UserGroupRoles.AsNoTracking().ToListAsync()));
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
            if (!await IsExistAsync(s => s.Id == id))
                return Result<GroupViewModel>.NotFound($"Group with ID ({id}) not found");
            var groupToView = _mapper.Map<GroupViewModel>(Exists.First());
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

        public async Task<Result<PostViewModel>> GetGroupPostByIdAsync(int postId, int groupId)
        {
            if (!await IsExistAsync(x => x.Id == groupId))
                return Result<PostViewModel>.NotFound($"Group with ID ({groupId}) not found");
            return await _postService.GetPostByIdAsync(postId, groupId);
        }

        public async Task<Result<List<PostViewModel>>> GetGroupPostsAsync(int groupId, int skip = 0, int count = 20)
        {
            if (!await IsExistAsync(x => x.Id == groupId))
                return Result<List<PostViewModel>>.NotFound($"Group with ID ({groupId}) not found");
            return await _postService.GetPostsByGroupIdAsync(groupId, skip, count);
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

        public async Task<Result<List<CommentViewModel>>> GetPostCommentsAsync(int groupId, int postId, int skip = 0, int count = 20)
        {
            if (!await IsExistAsync(x => x.Id == groupId))
                return Result<List<CommentViewModel>>.NotFound($"Group with ID ({groupId}) not found");

            var group = Exists.First();

            if (!await _postService.IsExistAsync(s => s.Id == postId))
                return Result<List<CommentViewModel>>.NotFound($"Post with ID ({postId}) not found");

            var post = _postService.Exists.First();

            if (post.GroupId != group.Id)
                return Result<List<CommentViewModel>>.NotFound($"This post isn't in this group");

            return await _commentService.GetCommentsByPostIdAsync(postId, skip, count);
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

        public async Task<Result<bool>> RemoveCommentAsync(int groupId, int postId, long commentId)
        {
            if (!await IsExistAsync(x => x.Id == groupId))
                return Result<bool>.NotFound($"Group with ID ({groupId}) not found");

            var group = Exists.First();

            if (!await _postService.IsExistAsync(s => s.Id == postId))
                return Result<bool>.NotFound($"Post with ID ({postId}) not found");

            var post = _postService.Exists.First();

            if (post.GroupId != group.Id)
                return Result<bool>.NotFound($"This post isn't in this group");

            return await _commentService.RemoveCommentAsync(commentId);
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

        public async Task<Result<bool>> RemoveGroupPostAsync(int postId, int groupId)
        {
            if (!await IsExistAsync(x => x.Id == groupId))
                return Result<bool>.NotFound($"Group with ID ({groupId}) not found");
            return await _postService.RemovePostAsync(postId, groupId);
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

        public async Task<Result<CommentViewModel>> UpdateCommentAsync(CommentEditModel model)
        {
            if (!await IsExistAsync(x => x.Id == model.GroupId))
                return Result<CommentViewModel>.NotFound($"Group with ID ({model.GroupId}) not found");

            var group = Exists.First();

            if (!await _postService.IsExistAsync(s => s.Id == model.PostId))
                return Result<CommentViewModel>.NotFound($"Post with ID ({model.PostId}) not found");

            var post = _postService.Exists.First();

            if (post.GroupId != group.Id)
                return Result<CommentViewModel>.NotFound($"This post isn't in this group");

            return await _commentService.UpdateCommentAsync(model);
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

        public async Task<Result<PostViewModel>> UpdateGroupPostAsync(PostEditModel model)
        {
            if (!await IsExistAsync(x => x.Id == model.GroupId))
                return Result<PostViewModel>.NotFound($"Group with ID ({model.GroupId}) not found");
            return await _postService.UpdatePostAsync(model);
        }
    }
}