using DUT.Application.Options;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Group;
using DUT.Application.ViewModels.Group.GroupMember;
using DUT.Application.ViewModels.Post;
using DUT.Application.ViewModels.Post.Comment;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface IGroupService : IBaseService<Group>
    {
        Task<Result<List<GroupViewModel>>> GetAllGroupsAsync(int count, int afterId);
        Task<Result<GroupViewModel>> GetGroupByIdAsync(int id);
        Task<Result<List<GroupViewModel>>> SearchGroupsAsync(SearchGroupOptions options);
        Task<Result<GroupViewModel>> CreateGroupAsync(GroupCreateModel model);
        Task<Result<GroupViewModel>> IncreaseCourseOfGroupAsync(int groupId);
        Task<Result<List<GroupMemberViewModel>>> GetGroupMembersAsync(int groupId, int afterId = int.MaxValue, int count = 20, int status = 0);
        Task<Result<GroupMemberViewModel>> GetGroupMemberByIdAsync(int groupId, int memberId);
        Task<Result<GroupMemberViewModel>> UpdateGroupMemberAsync(GroupMemberEditModel model);
        Task<Result<GroupMemberViewModel>> UpdateClassTeacherGroupAsync(GroupClassTeacherEditModel model);
        Task<Result<List<GroupInviteViewModel>>> GetGroupInvitesByGroupIdAsync(int groupId);
        Task<Result<GroupInviteViewModel>> CreateGroupInviteAsync(GroupInviteCreateModel model);
        Task<Result<GroupInviteViewModel>> UpdateGroupInviteAsync(GroupInviteEditModel model);
        Task<Result<bool>> RemoveGroupInviteAsync(int groupId, Guid groupInviteId);
        Task<Result<List<GroupShortViewModel>>> GetUserGroupsAsync(int userId);
        Task<Result<List<UserGroupRoleViewModel>>> GetAllGroupRolesAsync();

        Task<Result<List<PostViewModel>>> GetGroupPostsAsync(int groupId, int skip = 0, int count = 20);
        Task<Result<PostViewModel>> GetGroupPostByIdAsync(int postId, int groupId);
        Task<Result<PostViewModel>> CreateGroupPostAsync(PostCreateModel model);
        Task<Result<PostViewModel>> UpdateGroupPostAsync(PostEditModel model);
        Task<Result<bool>> RemoveGroupPostAsync(int postId, int groupId);

        Task<Result<List<CommentViewModel>>> GetPostCommentsAsync(int groupId, int postId, int skip = 0, int count = 20);
        Task<Result<CommentViewModel>> CreateCommentAsync(CommentCreateModel model);
        Task<Result<CommentViewModel>> UpdateCommentAsync(CommentEditModel model);
        Task<Result<bool>> RemoveCommentAsync(int groupId, int postId, long commentId);
    }
}