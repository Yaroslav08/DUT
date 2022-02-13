using AutoMapper;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Notification;
using DUT.Application.ViewModels.Session;
using DUT.Constants;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class SessionService : BaseService<Session>, ISessionService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public SessionService(DUTDbContext db, IMapper mapper, IIdentityService identityService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
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

        public async Task<Result<List<SessionViewModel>>> GetAllSessionsByUserIdAsync(int userId)
        {
            if (userId != _identityService.GetUserId())
                if (!_identityService.GetRoles().Contains(Roles.Admin))
                    return Result<List<SessionViewModel>>.Error("Access denited");

            var sessions = await _db.Sessions.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();
            return Result<List<SessionViewModel>>.SuccessWithData(_mapper.Map<List<SessionViewModel>>(sessions));
        }

        public async Task<Result<List<SessionViewModel>>> GetActiveSessionsByUserIdAsync(int userId)
        {
            if (userId != _identityService.GetUserId())
                if (!_identityService.GetRoles().Contains(Roles.Admin))
                    return Result<List<SessionViewModel>>.Error("Access denited");
            var sessions = await _db.Sessions.AsNoTracking().Where(x => x.UserId == userId && x.IsActive).ToListAsync();
            return Result<List<SessionViewModel>>.SuccessWithData(_mapper.Map<List<SessionViewModel>>(sessions));
        }
    }
}