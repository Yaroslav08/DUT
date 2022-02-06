using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Session;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface ISessionService : IBaseService<Session>
    {
        Task<Result<List<SessionViewModel>>> GetAllSessionsByUserIdAsync(int userId);
        Task<Result<List<SessionViewModel>>> GetActiveSessionsByUserIdAsync(int userId);
        Task<Result<SessionViewModel>> GetSessionByIdAsync(Guid sessionId);
    }
}
