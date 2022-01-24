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
    }
}