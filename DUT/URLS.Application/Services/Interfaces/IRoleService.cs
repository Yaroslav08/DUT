using URLS.Application.ViewModels;
using URLS.Application.ViewModels.RoleClaim;
using URLS.Domain.Models;
namespace URLS.Application.Services.Interfaces
{
    public interface IRoleService : IBaseService<Role>
    {
        Task<Result<List<RoleViewModel>>> GetAllRolesAsync();
        Task<Result<RoleViewModel>> GetRoleByIdAsync(int id, bool withClaims);
        Task<Result<RoleViewModel>> CreateRoleAsync(RoleCreateModel model);
        Task<Result<RoleViewModel>> UpdateRoleAsync(RoleEditModel model);
        Task<Result<RoleViewModel>> RemoveRoleAsync(int roleId);
    }
}