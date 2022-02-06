using DUT.Application.Options;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.User;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface IUserService : IBaseService<User>
    {
        Task<Result<UserViewModel>> CreateUserAsync(UserCreateModel model);
        Task<Result<List<UserShortViewModel>>> GetLastUsersAsync(int count);
        Task<Result<UserViewModel>> GetUserByIdAsync(int id);
        Task<Result<UserViewModel>> UpdateUsernameAsync(UsernameUpdateModel model);
        Task<Result<List<UserShortViewModel>>> SearchUsersAsync(SearchUserOptions searchUserOptions);
    }
}