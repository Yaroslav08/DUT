using DUT.Application.Services.Interfaces;
using System.Text.Json;

namespace DUT.Application.Services.Implementations
{
    public class SessionManager : ISessionManager, IAsyncDisposable
    {
        private readonly IList<string> _tokens;

        public SessionManager()
        {
            _tokens = new List<string>(5);
        }

        public SessionManager(string[] tokens)
        {
            _tokens = new List<string>(tokens);
        }

        public bool AddSession(string token)
        {
            if(_tokens.Contains(token))
                return false;
            _tokens.Add(token);
            return true;
        }

        public async ValueTask DisposeAsync()
        {
            await UploadContentAsync(_tokens);
        }

        public List<string> GetAllTokens()
        {
            return (List<string>)_tokens;
        }

        public bool IsActiveSession(string token)
        {
            if (_tokens.Contains(token))
                return true;
            return false;
        }

        public bool RemoveSession(string token)
        {
            if (_tokens.Contains(token))
            {
                _tokens.Remove(token);
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

        public bool AddRangeSessions(IEnumerable<string> tokens)
        {
            foreach (var token in tokens)
            {
                if (_tokens.Contains(token))
                {
                    continue;
                }
                else
                {
                    _tokens.Add(token);
                }
            }
            return true;
        }

        public bool RemoveRangeSession(IEnumerable<string> tokens)
        {
            foreach (var token in tokens)
            {
                if (_tokens.Contains(token))
                {
                    _tokens.Remove(token);
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
