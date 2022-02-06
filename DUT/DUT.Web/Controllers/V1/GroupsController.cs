using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Group;
using DUT.Application.ViewModels.Group.GroupMember;
using DUT.Application.ViewModels.Post;
using DUT.Application.ViewModels.Post.Comment;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class GroupsController : ApiBaseController
    {
        private readonly IGroupService _groupService;
        private readonly IIdentityService _identityService;
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        public GroupsController(IGroupService groupService, IIdentityService identityService, IPostService postService, ICommentService commentService)
        {
            _groupService = groupService;
            _identityService = identityService;
            _postService = postService;
            _commentService = commentService;
        }

        #region Groups

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] GroupCreateModel model)
        {
            return JsonResult(await _groupService.CreateGroupAsync(model));
        }

        [HttpPatch("{groupId}/increase-course")]
        public async Task<IActionResult> IncreaseCourseInGroup(int groupId)
        {
            return JsonResult(await _groupService.IncreaseCourseOfGroupAsync(groupId));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGroups(int afterId = int.MaxValue, int count = 20)
        {
            return JsonResult(await _groupService.GetAllGroupsAsync(afterId, count));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupById(int id)
        {
            return JsonResult(await _groupService.GetGroupByIdAsync(id));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchGroups(string name)
        {
            return JsonResult(await _groupService.SearchGroupsAsync(name));
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetGroupRoles()
        {
            return JsonResult(await _groupService.GetAllGroupRolesAsync());
        }

        #endregion

        #region Members

        [HttpGet("{groupId}/members")]
        public async Task<IActionResult> GetGroupMembers(int groupId, int afterId = int.MaxValue, int count = 20, int status = 0)
        {
            return JsonResult(await _groupService.GetGroupMembersAsync(groupId, afterId, count, status));
        }

        [HttpGet("{groupId}/members/{memberId}")]
        public async Task<IActionResult> GetGroupMember(int groupId, int memberId)
        {
            return JsonResult(await _groupService.GetGroupMemberByIdAsync(groupId, memberId));
        }

        [HttpPut("{groupId}/members/{memberId}")]
        public async Task<IActionResult> UpdateGroupMember(int groupId, int memberId, [FromBody] GroupMemberEditModel model)
        {
            model.GroupId = groupId;
            model.Id = memberId;
            return JsonResult(await _groupService.UpdateGroupMemberAsync(model));
        }

        #endregion

        #region Invites

        [HttpGet("{groupId}/invites")]
        public async Task<IActionResult> GetGroupInvites(int groupId)
        {
            return JsonResult(await _groupService.GetGroupInvitesByGroupIdAsync(groupId));
        }

        [HttpPost("{groupId}/invites")]
        public async Task<IActionResult> CreateGroupInvite(int groupId, [FromBody] GroupInviteCreateModel model)
        {
            model.GroupId = groupId;
            return JsonResult(await _groupService.CreateGroupInviteAsync(model));
        }

        [HttpPut("{groupId}/invites")]
        public async Task<IActionResult> UpdateGroupInvite(int groupId, [FromBody] GroupInviteEditModel model)
        {
            return JsonResult(await _groupService.UpdateGroupInviteAsync(model));
        }

        [HttpDelete("{groupId}/invites/{id}")]
        public async Task<IActionResult> RemoveGroupInvite(int groupId, Guid id)
        {
            return JsonResult(await _groupService.RemoveGroupInviteAsync(groupId, id));
        }

        #endregion

        #region Posts

        [HttpGet("{groupId}/posts")]
        public async Task<IActionResult> GetGroupPosts(int groupId, int offset = 0, int count = 20)
        {
            return JsonResult(await _postService.GetPostsByGroupIdAsync(groupId, offset, count));
        }

        [HttpPost("{groupId}/posts")]
        public async Task<IActionResult> CreatePost(int groupId, [FromBody] PostCreateModel model)
        {
            model.GroupId = groupId;
            return JsonResult(await _postService.CreatePostAsync(model));
        }

        [HttpPut("{groupId}/posts/{postId}")]
        public async Task<IActionResult> UpdatePost(int groupId, int postId, [FromBody] PostEditModel model)
        {
            model.Id = postId;
            model.GroupId = groupId;
            return JsonResult(await _postService.UpdatePostAsync(model));
        }

        [HttpGet("{groupId}/posts/{postId}")]
        public async Task<IActionResult> GetGroupPostById(int groupId, int postId)
        {
            return JsonResult(await _postService.GetPostByIdAsync(postId, groupId));
        }

        [HttpDelete("{groupId}/posts/{postId}")]
        public async Task<IActionResult> DeletePostById(int groupId, int postId)
        {
            return JsonResult(await _postService.RemovePostAsync(postId, groupId));
        }

        #endregion

        #region Comments

        [HttpGet("{groupId}/posts/{postId}/comments")]
        public async Task<IActionResult> GetPostComments(int groupId, int postId, int offset = 0, int count = 20)
        {
            return JsonResult(await _commentService.GetCommentsByPostIdAsync(postId, offset, count));
        }

        [HttpPost("{groupId}/posts/{postId}/comments")]
        public async Task<IActionResult> CreateComment(int groupId, int postId, [FromBody] CommentCreateModel model)
        {
            model.PostId = postId;
            return JsonResult(await _commentService.CreateCommentAsync(model));
        }

        [HttpPut("{groupId}/posts/{postId}/comments/{commentId}")]
        public async Task<IActionResult> UpdateComment(int groupId, int postId, long commentId, [FromBody] CommentEditModel model)
        {
            model.PostId = postId;
            return JsonResult(await _commentService.UpdateCommentAsync(model));
        }

        [HttpDelete("{groupId}/posts/{postId}/comments/{commentId}")]
        public async Task<IActionResult> RemoveComment(int groupId, int postId, long commentId)
        {
            return JsonResult(await _commentService.RemoveCommentAsync(commentId));
        }

        #endregion

    }
}
