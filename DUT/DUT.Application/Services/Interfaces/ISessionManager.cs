namespace DUT.Application.Services.Interfaces
{
    public interface ISessionManager
    {
        bool AddSession(int sessionId);
        bool AddRangeSessions(IEnumerable<int> sessionIds);
        bool RemoveSession(int sessionId);
        bool RemoveRangeSession(IEnumerable<int> sessionIds);
        bool IsActiveSession(int sessionId);
        List<int> GetAllSessionIds();
    }
}
