using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Group;
using DUT.Application.ViewModels.Group.GroupMember;

namespace DUT.Application.Services.Interfaces
{
    public interface IGroupService
    {
        Task<Result<List<GroupViewModel>>> GetAllGroupsAsync(int count, int afterId);
        Task<Result<GroupViewModel>> GetGroupByIdAsync(int id);
        Task<Result<List<GroupViewModel>>> SearchGroupsAsync(string name);
        Task<Result<GroupViewModel>> CreateGroupAsync(GroupCreateModel model);
        Task<Result<List<GroupMemberViewModel>>> GetGroupMembersAsync(int groupId, int afterId = int.MaxValue, int count = 20, int status = 0);
        Task<Result<GroupMemberViewModel>> GetGroupMemberByIdAsync(int groupId, int memberId);
        Task<Result<GroupMemberViewModel>> UpdateGroupMemberAsync(GroupMemberEditModel model);
        Task<Result<List<GroupInviteViewModel>>> GetGroupInvitesByGroupIdAsync(int groupId);
        Task<Result<GroupInviteViewModel>> CreateGroupInviteAsync(GroupInviteCreateModel model);
        Task<Result<GroupInviteViewModel>> UpdateGroupInviteAsync(GroupInviteEditModel model);
        Task<Result<bool>> RemoveGroupInviteAsync(int groupId, Guid groupInviteId);
    }
}