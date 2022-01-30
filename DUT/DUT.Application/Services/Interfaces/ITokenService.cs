using DUT.Application.ViewModels.Identity;
using DUT.Domain.Models;
namespace DUT.Application.Services.Interfaces
{
    public interface ITokenService
    {
        Task<JwtToken> GetUserTokenAsync(int userId, int sessionId, string authType);
        Task<JwtToken> GetUserTokenAsync(User user, int sessionId, string authType);
    }
}