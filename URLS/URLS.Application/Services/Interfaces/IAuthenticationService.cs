using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Identity;
using URLS.Application.ViewModels.User;

namespace URLS.Application.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Result<AuthenticationInfo>> RegisterAsync(RegisterViewModel model);
        Task<Result<JwtToken>> LoginByPasswordAsync(LoginCreateModel model);
        Task<Result<AuthenticationInfo>> ChangePasswordAsync(PasswordCreateModel model);
        Task<Result<UserViewModel>> BlockUserConfigAsync(BlockUserModel model);
        Task<Result<bool>> LogoutAsync();
        Task<Result<bool>> LogoutBySessionIdAsync(Guid id);
        Task<Result<bool>> LogoutAllAsync(int userId);
    }
}