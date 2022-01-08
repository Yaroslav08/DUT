using DUT.Application.Services.Interfaces;
using System.Text.Json;

namespace DUT.Application.Services.Implementations
{
    public class SessionManager : ISessionManager, IAsyncDisposable
    {
        private readonly IList<int> _sessionIds;

        public SessionManager()
        {
            _sessionIds = new List<int>(5);
        }

        public SessionManager(int[] sessionIds)
        {
            _sessionIds = new List<int>(sessionIds);
        }

        public bool AddSession(int sessionId)
        {
            if(_sessionIds.Contains(sessionId))
                return false;
            _sessionIds.Add(sessionId);
            return true;
        }

        public async ValueTask DisposeAsync()
        {
            await UploadContentAsync(_sessionIds);
        }

        public List<int> GetAllSessionIds()
        {
            return (List<int>)_sessionIds;
        }

        public bool IsActiveSession(int sessionId)
        {
            if (_sessionIds.Contains(sessionId))
                return true;
            return false;
        }

        public bool RemoveSession(int sessionId)
        {
            if (_sessionIds.Contains(sessionId))
            {
                _sessionIds.Remove(sessionId);
                return true;
            }
            return false;
        }

        private async Task<T> DownloadContentAsync<T>()
        {
            using var sr = new StreamReader(path);
            var content = await sr.ReadToEndAsync();
            return JsonSerializer.Deserialize<T>(content);
        }

        private async Task UploadContentAsync(object data)
        {
            using var sw = new StreamWriter(path);
            await sw.WriteAsync(JsonSerializer.Serialize(data));
        }

        public bool AddRangeSessions(IEnumerable<int> sessionIds)
        {
            foreach (int sessionId in sessionIds)
            {
                if (sessionIds.Contains(sessionId))
                {
                    continue;
                }
                else
                {
                    _sessionIds.Add(sessionId);
                }
            }
            return true;
        }

        public bool RemoveRangeSession(IEnumerable<int> sessionIds)
        {
            foreach (var sessionId in sessionIds)
            {
                if (sessionIds.Contains(sessionId))
                {
                    _sessionIds.Remove(sessionId);
                }
                else
                {
                    continue;
                }
            }
            return true;
        }

        private readonly string path = "sessions.json";
    }
}
