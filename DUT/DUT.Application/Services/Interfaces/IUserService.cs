using DUT.Application.Options;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.User;

namespace DUT.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<List<UserShortViewModel>>> GetLastUsersAsync(int count);
        Task<Result<UserViewModel>> GetUserByIdAsync(int id);
        Task<Result<List<UserShortViewModel>>> SearchUsersAsync(SearchUserOptions searchUserOptions);
    }
}