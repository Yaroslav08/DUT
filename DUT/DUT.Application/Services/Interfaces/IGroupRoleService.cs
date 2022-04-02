using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Group.GroupMember;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface IGroupRoleService : IBaseService<UserGroupRole>
    {
        Task<Result<List<UserGroupRoleViewModel>>> GetAllGroupRolesAsync();
    }
}