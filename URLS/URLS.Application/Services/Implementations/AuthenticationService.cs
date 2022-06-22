using AutoMapper;
using Extensions.DeviceDetector;
using Extensions.Password;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Helpers;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Identity;
using URLS.Application.ViewModels.Session;
using URLS.Application.ViewModels.User;
using URLS.Constants;
using URLS.Constants.Extensions;
using URLS.Constants.Localisation;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class AuthenticationService : Interfaces.IAuthenticationService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ISessionManager _sessionManager;
        private readonly ILocationService _locationService;
        private readonly IAppService _appService;
        private readonly ITokenService _tokenService;
        private readonly IDetector _detector;
        private readonly ICommonService _commonService;
        private readonly ILocalizeService _localizeService;

        public AuthenticationService(URLSDbContext db, IIdentityService identityService, ISessionManager sessionManager, ILocationService locationService, ITokenService tokenService, IDetector detector, IMapper mapper, ICommonService commonService, IAppService appService, ILocalizeService localizeService)
        {
            _db = db;
            _identityService = identityService;
            _sessionManager = sessionManager;
            _locationService = locationService;
            _tokenService = tokenService;
            _detector = detector;
            _mapper = mapper;
            _commonService = commonService;
            _appService = appService;
            _localizeService = localizeService;
        }

        public async Task<Result<UserViewModel>> BlockUserConfigAsync(BlockUserModel model)
        {
            if (!_identityService.IsAdministrator())
                return Result<UserViewModel>.Forbiden();

            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == model.UserId);
            if (user == null)
                return Result<UserViewModel>.NotFound(typeof(User).NotFoundMessage(model.UserId));
            var count = model.AccessFailedCount;
            if (count < 0 || count > 5)
                model.AccessFailedCount = 0;
            else
                user.AccessFailedCount = model.AccessFailedCount;

            user.LockoutEnabled = model.LockoutEnabled;
            user.LockoutEnd = model.LockoutEnd;

            user.PrepareToUpdate(_identityService);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return Result<UserViewModel>.SuccessWithData(_mapper.Map<UserViewModel>(user));
        }

        public async Task<Result<AuthenticationInfo>> ChangePasswordAsync(PasswordCreateModel model)
        {
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == _identityService.GetUserId());
            if (user == null)
                return Result<AuthenticationInfo>.NotFound(typeof(User).NotFoundMessage(_identityService.GetUserId()));

            if (!model.OldPassword.VerifyPasswordHash(user.PasswordHash))
                return Result<AuthenticationInfo>.Error(_localizeService.Get("passwordnotcomparer"));

            if (model.OldPassword == model.NewPassword)
                return Result<AuthenticationInfo>.Error("This passwords are match");

            user.PasswordHash = model.NewPassword.GeneratePasswordHash();

            user.PrepareToUpdate(_identityService);

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

        public async Task<Result<List<SocialViewModel>>> GetUserLoginsAsync(int userId)
        {
            var userLogins = await _db.UserLogins.AsNoTracking().Where(s => s.UserId == userId).ToListAsync();

            var socials = _mapper.Map<List<SocialViewModel>>(userLogins);

            foreach (var social in socials)
            {
                var lastSession = await _db.Sessions.AsNoTracking().OrderByDescending(s => s.CreatedBy).FirstOrDefaultAsync(s => s.Type == social.Provider);
                if (lastSession != null)
                    social.LastSigIn = lastSession.CreatedAt;
            }
            return Result<List<SocialViewModel>>.SuccessWithData(socials);
        }

        public async Task<Result<bool>> LinkSocialAsync(SocialCreateModel model)
        {
            var loginResult = await _commonService.IsExistWithResultsAsync<UserLogin>(s =>
                s.ExternalProvider == model.Scheme &&
                s.Email == model.Email &&
                s.Key == model.UniqId);

            if (loginResult.IsExist)
            {
                if (loginResult.Results.First().UserId == _identityService.GetUserId())
                    return Result<bool>.Success();
                return Result<bool>.Error("This account is linked to another account");
            }
            else
            {
                var newUserLogin = new UserLogin
                {
                    ExternalProvider = model.Scheme,
                    Email = model.Email,
                    Key = model.UniqId,
                    UserId = _identityService.GetUserId()
                };
                newUserLogin.PrepareToCreate(_identityService);
                await _db.UserLogins.AddAsync(newUserLogin);
                await _db.SaveChangesAsync();
                return Result<bool>.Success();
            }
        }

        public async Task<Result<JwtToken>> LoginByPasswordAsync(LoginCreateModel model)
        {
            var app = await _db.Apps
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.AppId == model.App.Id && x.AppSecret == model.App.Secret);

            if (app == null)
                return Result<JwtToken>.NotFound(typeof(App).NotFoundMessage(model.App.Id));

            if (!app.IsActive)
                return Result<JwtToken>.Error("App already unactive");

            if (!app.IsActiveByTime())
                return Result<JwtToken>.Error("App is expired");

            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Login == model.Login);
            if (user == null)
                return Result<JwtToken>.NotFound(typeof(User).NotFoundMessage(model.Login));

            if (!user.IsActivateAccount)
                return Result<JwtToken>.Error("Account not activated");

            if (user.LockoutEnabled)
            {
                if (user.IsLocked())
                {
                    return Result<JwtToken>.Error($"Your account has been locked up to {user.LockoutEnd.Value.ToString("HH:mm (dd.MM.yyyy)")}");
                }

                if (user.AccessFailedCount == 5)
                {
                    user.AccessFailedCount = 0;
                    user.LockoutEnd = DateTime.Now.AddHours(1);
                    var notifyLogin = NotificationsHelper.GetLockedNotification();
                    notifyLogin.UserId = user.Id;
                    _db.Users.Update(user);
                    await _db.Notifications.AddAsync(notifyLogin);
                    await _db.SaveChangesAsync();

                    return Result<JwtToken>.Error($"Account locked up to {user.LockoutEnd.Value.ToString("HH:mm (dd.MM.yyyy)")}");
                }
            }

            if (!model.Password.VerifyPasswordHash(user.PasswordHash))
            {
                user.AccessFailedCount++;
                _db.Users.Update(user);
                var loginAttemptNotify = NotificationsHelper.GetLoginAttemptNotification(model, _identityService.GetIP());
                loginAttemptNotify.UserId = user.Id;
                await _db.Notifications.AddAsync(loginAttemptNotify);
                await _db.SaveChangesAsync();
                return Result<JwtToken>.Error("Password is incorrect");
            }

            if (model.Client == null)
                model.Client = _detector.GetClientInfo();

            var location = await _locationService.GetIpInfoAsync(_identityService.GetIP());

            var appDb = new AppModel
            {
                Id = app.Id,
                Name = app.Name,
                ShortName = app.ShortName,
                Image = app.Image,
                Description = app.Description,
                Version = model.App.Version
            };

            var sessionId = Guid.NewGuid();

            var session = new Session
            {
                Id = sessionId,
                IsActive = true,
                App = appDb,
                Client = model.Client,
                Location = location,
                UserId = user.Id
            };

            var jwtToken = await _tokenService.GetUserTokenAsync(user.Id, sessionId, "pwd");

            session.Token = jwtToken.Token;
            session.ExpiredAt = jwtToken.ExpiredAt;
            session.Type = AuthScheme.Password;

            session.PrepareToCreate();

            var loginNotify = NotificationsHelper.GetLoginByPasswordNotification(session);
            loginNotify.UserId = user.Id;

            await _db.Notifications.AddAsync(loginNotify);

            await _db.Sessions.AddAsync(session);

            await _db.SaveChangesAsync();

            _sessionManager.AddSession(new TokenModel(jwtToken.Token, jwtToken.ExpiredAt));

            return Result<JwtToken>.SuccessWithData(jwtToken);
        }

        public async Task<Result<JwtToken>> LoginBySocialAsync(AuthenticateResult model, string scheme)
        {
            var appResult = await _appService.GetAppBySchemeAsync(scheme);
            if (!appResult.IsSuccess)
                return appResult.MapToNew<JwtToken>(null, null);

            var app = appResult.Data;

            if (!app.IsActive)
                return Result<JwtToken>.Error("App already unactive");

            if (!app.IsActiveByTime())
                return Result<JwtToken>.Error("App is expired");

            var claims = model.Principal.Identities.FirstOrDefault()?.Claims;

            var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

            var key = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var userLogin = await _db.UserLogins.Include(s => s.User)
                .AsNoTracking().FirstOrDefaultAsync(s =>
                    s.ExternalProvider == scheme &&
                    s.Email == email &&
                    s.Key == key);

            if (userLogin == null)
                return Result<JwtToken>.NotFound("No account linked to this social");

            var user = userLogin.User;

            if (!user.IsActivateAccount)
                return Result<JwtToken>.Error("Account not activated");

            var location = await _locationService.GetIpInfoAsync(_identityService.GetIP());

            var appDb = new AppModel
            {
                Id = app.Id,
                Name = app.Name,
                ShortName = app.ShortName,
                Image = app.Image,
                Description = app.Description,
                Version = null
            };

            var sessionId = Guid.NewGuid();

            var session = new Session
            {
                Id = sessionId,
                IsActive = true,
                App = appDb,
                Client = _detector.GetClientInfo(),
                Location = location,
                UserId = user.Id
            };

            var jwtToken = await _tokenService.GetUserTokenAsync(user.Id, sessionId, scheme);

            session.Token = jwtToken.Token;
            session.ExpiredAt = jwtToken.ExpiredAt;
            session.Type = scheme;

            session.PrepareToCreate();

            var loginNotify = NotificationsHelper.GetLoginBySocialNotification(session);
            loginNotify.UserId = user.Id;

            await _db.Notifications.AddAsync(loginNotify);

            await _db.Sessions.AddAsync(session);

            await _db.SaveChangesAsync();

            _sessionManager.AddSession(new TokenModel(jwtToken.Token, jwtToken.ExpiredAt));

            return Result<JwtToken>.SuccessWithData(jwtToken);
        }

        public async Task<Result<bool>> LogoutAllAsync(int userId)
        {
            var sessions = await _db.Sessions.Where(x => x.IsActive && x.UserId == userId).ToListAsync();

            var tokens = sessions.Select(x => x.Token);

            var currentSessionId = _identityService.GetCurrentSessionId();

            sessions.ForEach(session =>
            {
                session.IsActive = false;
                session.DeactivatedBySessionId = currentSessionId;
                session.DeactivatedAt = DateTime.Now;
                session.PrepareToUpdate(_identityService);
            });

            _db.Sessions.UpdateRange(sessions);

            var notify = NotificationsHelper.GetAllLogoutNotification();
            notify.UserId = userId;
            await _db.Notifications.AddAsync(notify);
            await _db.SaveChangesAsync();

            _sessionManager.RemoveRangeSession(tokens);

            return Result<bool>.Success();
        }

        public async Task<Result<bool>> LogoutAsync()
        {
            var token = _identityService.GetBearerToken();
            if (!_sessionManager.IsActiveSession(token))
                return Result<bool>.Error("Session is already expired");

            var session = await _db.Sessions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == _identityService.GetCurrentSessionId());
            if (session == null)
                return Result<bool>.NotFound(typeof(Session).NotFoundMessage(_identityService.GetCurrentSessionId()));

            var now = DateTime.Now;
            session.IsActive = false;
            session.DeactivatedAt = now;
            session.DeactivatedBySessionId = _identityService.GetCurrentSessionId();
            session.PrepareToUpdate(_identityService);
            _db.Sessions.Update(session);
            await _db.SaveChangesAsync();

            _sessionManager.RemoveSession(token);

            var notify = NotificationsHelper.GetLogoutNotification(session);
            notify.UserId = _identityService.GetUserId();
            await _db.Notifications.AddAsync(notify);
            await _db.SaveChangesAsync();

            return Result<bool>.SuccessWithData(true);
        }

        public async Task<Result<bool>> LogoutBySessionIdAsync(Guid id)
        {
            var sessionToClose = await _db.Sessions.FirstOrDefaultAsync(x => x.Id == id);
            if (sessionToClose == null)
                return Result<bool>.NotFound(typeof(Session).NotFoundMessage(id));
            var now = DateTime.Now;
            sessionToClose.IsActive = false;
            sessionToClose.DeactivatedAt = now;
            sessionToClose.DeactivatedBySessionId = _identityService.GetCurrentSessionId();
            sessionToClose.PrepareToUpdate(_identityService);
            _db.Sessions.Update(sessionToClose);
            var notify = NotificationsHelper.GetLogoutNotification(sessionToClose);
            notify.UserId = sessionToClose.UserId;
            await _db.Notifications.AddAsync(notify);
            await _db.SaveChangesAsync();
            _sessionManager.RemoveSession(sessionToClose.Token);
            return Result<bool>.Success();
        }

        public async Task<Result<AuthenticationInfo>> RegisterAsync(RegisterViewModel model)
        {
            if (await _commonService.IsExistAsync<User>(s => s.Login == model.Login))
                return Result<AuthenticationInfo>.Error("Login is busy");

            var groupInvite = await _db.GroupInvites.AsNoTracking().FirstOrDefaultAsync(s => s.CodeJoin == model.Code);
            if (groupInvite == null)
                return Result<AuthenticationInfo>.Error("Code isn't exist");

            if (!groupInvite.IsActive)
                return Result<AuthenticationInfo>.Error("Code already unactive");

            if (!groupInvite.IsActiveByTime())
                return Result<AuthenticationInfo>.Error("Code is expired");

            var newUser = new User(model.FirstName, null, model.LastName, model.Login, Generator.GetUsername());
            newUser.PasswordHash = model.Password.GeneratePasswordHash();
            newUser.NotificationSettings = new NotificationSettings
            {
                AcceptedInGroup = true,
                ChangePassword = true,
                Logout = true,
                NewLogin = true,
                NewPost = true,
                Welcome = true
            };
            newUser.IsActivateAccount = false;

            newUser.PrepareToCreate();

            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();

            var role = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(s => s.Name == Roles.Student);

            var userRole = new UserRole
            {
                UserId = newUser.Id,
                RoleId = role.Id
            };
            userRole.PrepareToCreate();

            await _db.UserRoles.AddAsync(userRole);
            await _db.SaveChangesAsync();

            var groupRoleStudent = await _db.UserGroupRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UniqId == UserGroupRoles.UniqIds.Student);

            var groupStudent = new UserGroup
            {
                UserId = newUser.Id,
                GroupId = groupInvite.GroupId,
                IsAdmin = false,
                Status = UserGroupStatus.New,
                Title = "Студент",
                UserGroupRoleId = groupRoleStudent.Id
            };

            groupStudent.PrepareToCreate();

            await _db.UserGroups.AddAsync(groupStudent);
            await _db.SaveChangesAsync();

            return Result<AuthenticationInfo>.Created();
        }

        public async Task<Result<bool>> UnlinkSocialAsync(int socialId)
        {
            var userLoginToRemove = await _db.UserLogins.AsNoTracking().FirstOrDefaultAsync(s => s.Id == socialId);
            if (userLoginToRemove == null)
                return Result<bool>.NotFound();

            if (userLoginToRemove.UserId != _identityService.GetUserId())
                return Result<bool>.Forbiden();

            _db.UserLogins.Remove(userLoginToRemove);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }
    }
}