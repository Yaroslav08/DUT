using DUT.Application.ViewModels;
using DUT.Application.ViewModels.RoleClaim;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface IClaimService : IBaseService<Claim>
    {
        Task<Result<List<ClaimViewModel>>> GetAllClaimsAsync();
        Task<Result<List<ClaimViewModel>>> GetClaimsByRoleIdAsync(int roleId);
        Task<Result<ClaimViewModel>> UpdateClaimAsync(ClaimEditModel model);
    }
}