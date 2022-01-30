using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Session;

namespace DUT.Application.Services.Interfaces
{
    public interface ISessionService
    {
        Task<Result<List<SessionViewModel>>> GetAllSessionsByUserIdAsync(int userId);
        Task<Result<List<SessionViewModel>>> GetActiveSessionsByUserIdAsync(int userId);
        Task<Result<SessionViewModel>> GetSessionByIdAsync(int sessionId);
    }
}
