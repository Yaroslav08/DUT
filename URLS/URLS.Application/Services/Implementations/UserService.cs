using AutoMapper;
using Extensions.Password;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Helpers;
using URLS.Application.Options;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group;
using URLS.Application.ViewModels.RoleClaim;
using URLS.Application.ViewModels.Session;
using URLS.Application.ViewModels.User;
using URLS.Application.ViewModels.User.UserInfo;
using URLS.Constants;
using URLS.Constants.APIResponse;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;
namespace URLS.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICommonService _commonService;
        private readonly IIdentityService _identityService;
        public UserService(URLSDbContext db, IMapper mapper, IIdentityService identityService, ICommonService commonService)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
            _commonService = commonService;
        }

        public async Task<Result<UserViewModel>> CreateUserAsync(UserCreateModel model)
        {
            if (await _commonService.IsExistAsync<User>(s => s.Login == model.Login))
                return Result<UserViewModel>.Error("Login is busy");

            if (!await _db.Roles.AsNoTracking().AnyAsync(s => s.Id == model.RoleId))
                return Result<UserViewModel>.NotFound("Role not found");

            if (!string.IsNullOrEmpty(model.UserName))
                if (await _commonService.IsExistAsync<User>(s => s.UserName == model.UserName))
                    return Result<UserViewModel>.Error("Username is busy");

            var newUser = new User(model.FirstName, model.MiddleName, model.LastName, model.Login, null);
            newUser.UserName = model.UserName ?? Generator.GetUsername();
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
            newUser.IsActivateAccount = true;
            newUser.PrepareToCreate(_identityService);
            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();

            var userRole = new UserRole
            {
                UserId = newUser.Id,
                RoleId = model.RoleId
            };
            userRole.PrepareToCreate(_identityService);

            await _db.UserRoles.AddAsync(userRole);

            var notify = NotificationsHelper.GetWelcomeNotification();
            notify.UserId = newUser.Id;

            await _db.Notifications.AddAsync(notify);
            await _db.SaveChangesAsync();

            return Result<UserViewModel>.Created(_mapper.Map<UserViewModel>(newUser)); ;
        }

        public async Task<Result<UserFullViewModel>> GetFullInfoUserByIdAsync(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return Result<UserFullViewModel>.NotFound("User not found");
            var userToView = _mapper.Map<UserFullViewModel>(user);

            userToView.Block = new BlockInfo
            {
                AccessFailedCount = user.AccessFailedCount,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd
            };

            var sessions = await _db.Sessions.AsNoTracking().Where(x => x.UserId == id).ToListAsync();

            if (sessions != null && sessions.Count > 0)
                userToView.Session = new SessionInfo
                {
                    Sessions = _mapper.Map<List<SessionViewModel>>(sessions),
                    TotalSessions = sessions.Count,
                    ActiveSessions = sessions.Count(s => s.IsActive)
                };

            var userRoles = await _db.UserRoles.AsNoTracking().Include(s => s.Role).Where(s => s.UserId == id).Select(s => s.Role).ToListAsync();
            if (userRoles != null && userRoles.Count > 0)
                userToView.Role = new RoleInfo
                {
                    Roles = _mapper.Map<List<RoleViewModel>>(userRoles)
                };

            var groups = await _db.UserGroups.AsNoTracking().Include(s => s.Group).Where(s => s.UserId == id).Select(s => s.Group).ToListAsync();
            if (groups != null && groups.Count > 0)
                userToView.Group = new GroupInfo
                {
                    Groups = _mapper.Map<List<GroupViewModel>>(groups)
                };

            return Result<UserFullViewModel>.SuccessWithData(userToView);
        }

        public async Task<Result<List<UserShortViewModel>>> GetLastUsersAsync(int count)
        {
            var lastUsers = await _db.Users
                .AsNoTracking()
                .OrderByDescending(s => s.JoinAt)
                .Take(count)
                .ToListAsync();
            return Result<List<UserShortViewModel>>.SuccessWithData(_mapper.Map<List<UserShortViewModel>>(lastUsers));
        }

        public async Task<Result<List<UserShortViewModel>>> GetTeachersAsync(int offset = 0, int count = 20)
        {
            var teachers = await _db.UserRoles
                .Where(s => s.RoleId == 4)
                .Include(s => s.User)
                .OrderBy(s => s.UserId)
                .Skip(offset).Take(count)
                .Select(s => new UserShortViewModel
                {
                    Id = s.User.Id,
                    FirstName = s.User.FirstName,
                    LastName = s.User.LastName,
                    UserName = s.User.UserName,
                    Image = s.User.Image,
                    JoinAt = s.User.JoinAt
                })
                .ToListAsync();

            var totalCount = await _db.UserRoles.CountAsync(s => s.RoleId == 4);

            return Result<List<UserShortViewModel>>.SuccessList(teachers, Meta.FromMeta(totalCount, offset, count));
        }

        public async Task<Result<UserViewModel>> GetUserByIdAsync(int id)
        {
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return Result<UserViewModel>.NotFound("User not found");
            return Result<UserViewModel>.SuccessWithData(_mapper.Map<UserViewModel>(user));
        }

        public async Task<Result<List<UserShortViewModel>>> SearchUsersAsync(SearchUserOptions searchUserOptions)
        {
            searchUserOptions.PrepareOptions();

            IQueryable<User> query = _db.Users;

            query = query.AsNoTracking();

            query = query.Skip(searchUserOptions.Offset).Take(searchUserOptions.Count);

            if (!string.IsNullOrEmpty(searchUserOptions.FirstName))
                query = query.Where(x => x.FirstName.Contains(searchUserOptions.FirstName));

            if (!string.IsNullOrEmpty(searchUserOptions.LastName))
                query = query.Where(x => x.LastName.Contains(searchUserOptions.LastName));

            //Other filters


            var result = await query.OrderBy(x => x.Id).ToListAsync();

            return Result<List<UserShortViewModel>>.SuccessWithData(_mapper.Map<List<UserShortViewModel>>(result));
        }

        public async Task<Result<NotificationSettings>> UpdateNotificationSettingsAsync(int userId, NotificationSettings notificationSettings)
        {
            if (!_identityService.IsAdministrator())
                if (userId != _identityService.GetUserId())
                    return Result<NotificationSettings>.Error("Access denited");

            var userToUpdate = await _db.Users.FindAsync(userId);

            userToUpdate.NotificationSettings = notificationSettings;

            userToUpdate.PrepareToUpdate(_identityService);

            _db.Users.Update(userToUpdate);
            await _db.SaveChangesAsync();

            return Result<NotificationSettings>.SuccessWithData(notificationSettings);
        }

        public async Task<Result<UserViewModel>> UpdateUsernameAsync(UsernameUpdateModel model)
        {
            if (model.UserId != _identityService.GetUserId())
                if (!_identityService.GetRoles().Contains(Roles.Admin))
                    return Result<UserViewModel>.Error("Access denited");

            var userToUpdate = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.UserId);
            if (userToUpdate == null)
                return Result<UserViewModel>.NotFound("User by id not found");

            if (userToUpdate.UserName == model.Username)
                return Result<UserViewModel>.Error("Username equals current you");

            if (await _commonService.IsExistAsync<User>(s => s.UserName == model.Username))
                return Result<UserViewModel>.Error("Username is already busy");

            userToUpdate.UserName = model.Username;
            userToUpdate.PrepareToUpdate(_identityService);
            _db.Users.Update(userToUpdate);
            await _db.SaveChangesAsync();
            return Result<UserViewModel>.SuccessWithData(_mapper.Map<UserViewModel>(userToUpdate));
        }
    }
}