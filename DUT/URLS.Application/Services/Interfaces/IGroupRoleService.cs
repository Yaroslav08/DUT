using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group.GroupMember;
using URLS.Domain.Models;

namespace URLS.Application.Services.Interfaces
{
    public interface IGroupRoleService : IBaseService<UserGroupRole>
    {
        Task<Result<List<UserGroupRoleViewModel>>> GetAllGroupRolesAsync();
    }
}