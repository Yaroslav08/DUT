using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Identity;
using DUT.Application.ViewModels.User;
using DUT.Infrastructure.Data.Context;
using Extensions.DeviceDetector;
using Extensions.Password;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly DUTDbContext _db;
        private readonly IIdentityService _identityService;
        private readonly IDetector _detector;
        public AuthenticationService(DUTDbContext db, IDetector detector, IIdentityService identityService)
        {
            _db = db;
            _detector = detector;
            _identityService = identityService;
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
            await _db.Notifications.AddAsync(new Domain.Models.Notification
            {
                CreatedAt = DateTime.Now,
                CreatedBy = _identityService.GetIdentityData(),
                CreatedFromIP = model.IP,
                Content = "Увага! Ваш пароль було змінено",
                ImageUrl = "https://thumbs.dreamstime.com/b/lock-login-password-safe-security-icon-vector-illustration-flat-design-lock-login-password-safe-security-icon-vector-illustration-131742100.jpg",
                IsImportant = true,
                ReadAt = null,
                Title = "Пароль було змінено",
                Type = Domain.Models.NotificationType.ChangePassword
            });
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
            throw new NotImplementedException();
        }

        public async Task<Result<AuthenticationInfo>> RegisterAsync(RegisterViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
