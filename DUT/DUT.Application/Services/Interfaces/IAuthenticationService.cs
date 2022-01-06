using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Identity;
using DUT.Application.ViewModels.User;

namespace DUT.Application.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Result<AuthenticationInfo>> RegisterAsync(RegisterViewModel model);
        Task<Result<AuthenticationInfo>> LoginAsync(LoginCreateModel model);
        Task<Result<AuthenticationInfo>> ChangePasswordAsync(PasswordCreateModel model);
        Task<Result<bool>> LogoutAsync();
    }
}