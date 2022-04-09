using URLS.Application.ViewModels.Identity;
using URLS.Domain.Models;
namespace URLS.Application.Services.Interfaces
{
    public interface ITokenService
    {
        Task<JwtToken> GetUserTokenAsync(int userId, Guid sessionId, string authType);
        Task<JwtToken> GetUserTokenAsync(User user, Guid sessionId, string authType);
    }
}