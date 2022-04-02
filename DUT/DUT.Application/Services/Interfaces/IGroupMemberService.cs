using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Group.GroupMember;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface IGroupMemberService : IBaseService<UserGroup>
    {
        Task<Result<List<GroupMemberViewModel>>> GetGroupMembersAsync(int groupId, int afterId = int.MaxValue, int count = 20, int status = 0);
        Task<Result<GroupMemberViewModel>> GetGroupMemberByIdAsync(int groupId, int memberId);
        Task<Result<GroupMemberViewModel>> UpdateGroupMemberAsync(GroupMemberEditModel model);
        Task<Result<bool>> AcceptAllNewGroupMembersAsync(int groupId);
        Task<Result<bool>> AcceptNewGroupMemberAsync(int groupId, int groupMemberId);
    }
}