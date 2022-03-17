using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Session;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface ISessionService : IBaseService<Session>
    {
        Task<Result<List<SessionViewModel>>> GetAllSessionsByUserIdAsync(int userId, int q = 0, int offset = 0, int limit = 20);
        Task<Result<SessionViewModel>> GetSessionByIdAsync(Guid sessionId);
        Task<Result<bool>> CloseSessionByIdAsync(Guid sessionId);
        Task<Result<bool>> CloseAllSessionsAsync(int userId, bool withCurrent = true);
    }
}