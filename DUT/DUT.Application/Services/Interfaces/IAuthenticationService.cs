using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Identity;

namespace DUT.Application.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Result<AuthenticationInfo>> LoginAsync(LoginCreateModel model);
        Task<Result<bool>> LogoutAsync();
    }
}