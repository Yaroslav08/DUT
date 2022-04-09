using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group;
using URLS.Domain.Models;

namespace URLS.Application.Services.Interfaces
{
    public interface IGroupInviteService : IBaseService<GroupInvite>
    {
        Task<Result<List<GroupInviteViewModel>>> GetGroupInvitesByGroupIdAsync(int groupId);
        Task<Result<GroupInviteViewModel>> CreateGroupInviteAsync(GroupInviteCreateModel model);
        Task<Result<GroupInviteViewModel>> UpdateGroupInviteAsync(GroupInviteEditModel model);
        Task<Result<bool>> RemoveGroupInviteAsync(int groupId, Guid groupInviteId);
    }
}