using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Group;
using DUT.Application.ViewModels.Group.GroupMember;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers.Api.V1
{
    [ApiVersion("1.0")]
    public class GroupsController : ApiBaseController
    {
        private readonly IGroupService _groupService;
        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] GroupCreateModel model)
        {
            return JsonResult(await _groupService.CreateGroupAsync(model));
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

        [HttpGet("search")]
        public async Task<IActionResult> SearchGroups(string name)
        {
            return JsonResult(await _groupService.SearchGroupsAsync(name));
        }
    }
}
