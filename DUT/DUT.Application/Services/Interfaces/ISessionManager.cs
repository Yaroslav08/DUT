namespace DUT.Application.Services.Interfaces
{
    public interface ISessionManager
    {
        bool AddSession(string token);
        bool AddRangeSessions(IEnumerable<string> tokens);
        bool RemoveSession(string token);
        bool RemoveRangeSession(IEnumerable<string> tokens);
        bool IsActiveSession(string token);
        List<string> GetAllTokens();
    }
}