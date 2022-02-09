using DUT.Application.ViewModels.Identity;

namespace DUT.Application.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(string claimType, string claimValue, object data = null);
        bool HasPermission(string claimType, string claimValue, UserIdentity currentUser = null);
    }
}
