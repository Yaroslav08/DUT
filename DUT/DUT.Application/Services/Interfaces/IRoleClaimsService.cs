using DUT.Application.ViewModels;
using DUT.Application.ViewModels.RoleClaim;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface IRoleClaimsService : IBaseService<Role>
    {
        Task<Result<List<RoleViewModel>>> GetAllRolesAsync();
        Task<Result<RoleViewModel>> GetRoleByIdAsync(int id, bool withClaims = false);
        Task<Result<RoleViewModel>> CreateRoleAsync(RoleCreateModel model);
        Task<Result<RoleViewModel>> UpdateRoleAsync(RoleEditModel model);
        Task<Result<RoleViewModel>> RemoveRoleAsync(int id);

        Task<Result<List<ClaimViewModel>>> GetClaimsAsync();
        Task<Result<ClaimViewModel>> UpdateClaimAsync(ClaimEditModel model);
    }
}