using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Identity;
using DUT.Application.ViewModels.User;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface IAuthenticationService : IBaseService<User>
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