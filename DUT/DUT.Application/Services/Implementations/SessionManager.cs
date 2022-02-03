using DUT.Application.Services.Interfaces;
using DUT.Infrastructure.Data.Context;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace DUT.Application.Services.Implementations
{
    public class SessionManager : ISessionManager
    {
        private readonly IList<string> _tokens;

        private IList<string> GetActualTokensFromDb(DUTDbContext db)
        {
            var sessions = db.Sessions.Where(s => s.IsActive).Select(s => s.Token).ToList();
            if (sessions == null || !sessions.Any())
                return new List<string>();
            return sessions;
        }

        public SessionManager(IServiceScopeFactory _serviceScopeFactory)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<DUTDbContext>();

            _tokens = GetActualTokensFromDb(dbContext);
        }

        public bool AddSession(string token)
        {
            if (_tokens.Contains(token))
                return false;
            _tokens.Add(token);
            return true;
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
    }
}
