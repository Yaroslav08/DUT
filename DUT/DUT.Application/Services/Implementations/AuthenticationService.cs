using DUT.Application.Helpers;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Identity;
using DUT.Application.ViewModels.User;
using DUT.Constants;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Extensions.DeviceDetector;
using Extensions.Password;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class AuthenticationService : BaseService<User>, IAuthenticationService
    {
        private readonly DUTDbContext _db;
        private readonly IIdentityService _identityService;
        private readonly ISessionManager _sessionManager;
        private readonly IDetector _detector;
        public AuthenticationService(DUTDbContext db, IDetector detector, IIdentityService identityService, ISessionManager sessionManager) : base(db)
        {
            _db = db;
            _detector = detector;
            _identityService = identityService;
            _sessionManager = sessionManager;
        }

        public async Task<Result<AuthenticationInfo>> ChangePasswordAsync(PasswordCreateModel model)
        {
            var user = await _db.Users.AsNoTracking().SingleOrDefaultAsync(s => s.Id == model.UserId);
            if (user == null)
                return Result<AuthenticationInfo>.NotFound("User not found");

            if (model.OldPassword.VerifyPasswordHash(user.PasswordHash))
                return Result<AuthenticationInfo>.Error("Password not comparer");

            if (model.OldPassword == model.NewPassword)
                return Result<AuthenticationInfo>.Error("This passwords are match");

            user.PasswordHash = model.NewPassword.GeneratePasswordHash();
            user.LastUpdatedAt = DateTime.Now;
            user.LastUpdatedBy = _identityService.GetIdentityData();
            user.LastUpdatedFromIP = model.IP;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            var notification = NotificationsHelper.GetChangePasswordNotification();
            notification.UserId = user.Id;
            await _db.Notifications.AddAsync(notification);
            await _db.SaveChangesAsync();
            return Result<AuthenticationInfo>.SuccessWithData(new AuthenticationInfo
            {
                User = user,
            });
        }

        public async Task<Result<AuthenticationInfo>> LoginAsync(LoginCreateModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> LogoutAsync()
        {
            var token = _identityService.GetBearerToken();
            if (!_sessionManager.IsActiveSession(token))
                return Result<bool>.Error("Session is already expired");

            var session = await _db.Sessions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == _identityService.GetCurrentSessionId());
            if (session == null)
                return Result<bool>.Error("Session not found");

            var now = DateTime.Now;
            session.IsActive = false;
            session.DeactivatedAt = now;
            session.DeactivatedBySessionId = _identityService.GetCurrentSessionId();
            session.LastUpdatedFromIP = Defaults.IP;
            session.LastUpdatedBy = Defaults.CreatedBy;
            session.LastUpdatedAt = now;
            _db.Sessions.Update(session);
            await _db.SaveChangesAsync();

            var notify = NotificationsHelper.GetLogoutNotification(session);

            await _db.Notifications.AddAsync(notify);
            await _db.SaveChangesAsync();

            _sessionManager.RemoveSession(token);
            return Result<bool>.SuccessWithData(true);
        }

        public async Task<Result<AuthenticationInfo>> RegisterAsync(RegisterViewModel model)
        {
            if (await IsExistAsync(s => s.Login == model.Login))
                return Result<AuthenticationInfo>.Error("Login is busy");

            var groupInvite = await _db.GroupInvites.AsNoTracking().SingleOrDefaultAsync(s => s.CodeJoin == model.Code);
            if (groupInvite == null)
                return Result<AuthenticationInfo>.Error("Code isn't exist");

            if (groupInvite.IsActive)
                return Result<AuthenticationInfo>.Error("Code already unactive");

            if (!groupInvite.IsActiveByTime())
                return Result<AuthenticationInfo>.Error("Code is expired");

            var newUser = new User(model.FirstName, null, model.LastName, model.Login, Generator.GetUsername());

            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();

            await _db.UserRoles.AddAsync(new UserRole
            {
                UserId = newUser.Id,
                RoleId = 1
            });
            await _db.SaveChangesAsync();

            var groupStudent = new UserGroup
            {
                UserId = newUser.Id,
                GroupId = groupInvite.GroupId,
                IsAdmin = false,
                Status = UserGroupStatus.New,
                Title = "Студент"
            };

            await _db.UserGroups.AddAsync(groupStudent);
            await _db.SaveChangesAsync();

            return Result<AuthenticationInfo>.Success();
        }
    }
}
