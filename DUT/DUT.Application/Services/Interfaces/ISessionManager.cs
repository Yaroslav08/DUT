using DUT.Application.ViewModels.Session;

namespace DUT.Application.Services.Interfaces
{
    public interface ISessionManager
    {
        bool AddSession(TokenModel token);
        bool AddRangeSessions(IEnumerable<TokenModel> tokens);
        bool RemoveSession(string token);
        bool RemoveRangeSession(IEnumerable<string> tokens);
        bool IsActiveSession(string token);
        IList<TokenModel> GetAllTokens();
    }
}