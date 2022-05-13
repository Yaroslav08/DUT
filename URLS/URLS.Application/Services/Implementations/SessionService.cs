using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Session;
using URLS.Constants;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;
namespace URLS.Application.Services.Implementations
{
    public class SessionService : ISessionService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ISessionManager _sessionManager;
        public SessionService(URLSDbContext db, IMapper mapper, IIdentityService identityService, ISessionManager sessionManager)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
            _sessionManager = sessionManager;
        }

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(Guid sessionId)
        {
            var session = await _db.Sessions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == sessionId);
            if (session == null)
                return Result<SessionViewModel>.NotFound("Session not found");

            if (session.UserId != _identityService.GetUserId())
                if (!_identityService.GetRoles().Contains(Roles.Admin))
                    return Result<SessionViewModel>.Error("Access denited");

            return Result<SessionViewModel>.SuccessWithData(_mapper.Map<SessionViewModel>(session));
        }

        public async Task<Result<List<SessionViewModel>>> GetAllSessionsByUserIdAsync(int userId, int q = 0, int offset = 0, int limit = 20)
        {
            if (userId != _identityService.GetUserId())
                if (!_identityService.GetRoles().Contains(Roles.Admin))
                    return Result<List<SessionViewModel>>.Error("Access denited");

            if (offset < 0 || limit < 0 && (q != 0 || q != 1 || q != 2))
                return Result<List<SessionViewModel>>.Error("Please check enter data");

            var query = _db.Sessions
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Skip(offset).Take(limit);

            if (q == 0)
                query = query.Where(x => x.IsActive);
            if (q == 1)
                query = query.Where(x => !x.IsActive);

            var sessions = await query.ToListAsync();

            var sessionsToView = SortSessions(sessions);

            return Result<List<SessionViewModel>>.SuccessWithData(sessionsToView);
        }

        private List<SessionViewModel> SortSessions(List<Session> sessions)
        {
            var sortedSessions = new List<SessionViewModel>();
            if (sessions == null || sessions.Count == 0)
                return sortedSessions;

            var activeSessions = sessions.Where(x => x.IsActive)
                .OrderByDescending(x => x.CreatedAt).Select(x => new SessionViewModel
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    IsActive = x.IsActive,
                    App = x.App,
                    Client = x.Client,
                    DeactivatedAt = x.DeactivatedAt,
                    Location = x.Location
                });
            sortedSessions.AddRange(activeSessions);

            var unActiveSessions = sessions.Where(s => !s.IsActive)
                .OrderByDescending(x => x.DeactivatedAt).Select(x => new SessionViewModel
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    IsActive = x.IsActive,
                    App = x.App,
                    Client = x.Client,
                    DeactivatedAt = x.DeactivatedAt,
                    Location = x.Location
                });
            sortedSessions.AddRange(unActiveSessions);

            var currentToken = _identityService.GetBearerToken();
            var currentSessionId = sessions.FirstOrDefault(s => s.Token == currentToken).Id;

            for (int i = 0; i < sortedSessions.Count; i++)
            {
                if (sortedSessions[i].Id == currentSessionId)
                {
                    sortedSessions[i].IsCurrent = true;
                }
            }
            var currentSession = sortedSessions.FirstOrDefault(x => x.IsCurrent);
            sortedSessions.Remove(currentSession);
            sortedSessions.Insert(0, currentSession);
            return sortedSessions;
        }

        public async Task<Result<bool>> CloseSessionByIdAsync(Guid sessionId)
        {
            var session = await _db.Sessions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == sessionId);
            if (session == null)
                return Result<bool>.NotFound("Session not found");

            if (session.UserId != _identityService.GetUserId())
                if (_identityService.GetRoles().Any(s => s == Roles.Admin))
                    return Result<bool>.Error("Access denited");

            if (!session.IsActive && !_sessionManager.IsActiveSession(session.Token))
                return Result<bool>.Error("Session is already closed");

            _sessionManager.RemoveSession(session.Token);
            session.IsActive = false;
            session.DeactivatedAt = DateTime.Now;
            session.DeactivatedBySessionId = _identityService.GetCurrentSessionId();
            session.PrepareToUpdate(_identityService);
            _db.Sessions.Update(session);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<bool>> CloseAllSessionsAsync(int userId, bool withCurrent = true)
        {
            var currentUserId = _identityService.GetUserId();

            if (userId != currentUserId)
                if (_identityService.GetRoles().Any(s => s == Roles.Admin))
                    return Result<bool>.Error("Access denited");

            var sessionsToClose = await _db.Sessions.AsNoTracking().Where(x => x.IsActive && x.UserId == userId).ToListAsync();

            if (sessionsToClose == null || sessionsToClose.Count == 0)
                return Result<bool>.Success();

            var currentToken = _identityService.GetBearerToken();
            var now = DateTime.Now;
            var currentSessionId = _identityService.GetCurrentSessionId();

            if (!withCurrent)
                sessionsToClose.Remove(sessionsToClose.FirstOrDefault(s => s.Token == currentToken));

            sessionsToClose.ForEach(x =>
            {
                x.IsActive = false;
                x.DeactivatedAt = now;
                x.DeactivatedBySessionId = currentSessionId;
                x.PrepareToUpdate(_identityService);
            });

            _sessionManager.RemoveRangeSession(sessionsToClose.Select(x => x.Token));
            _db.Sessions.UpdateRange(sessionsToClose);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }
    }
}