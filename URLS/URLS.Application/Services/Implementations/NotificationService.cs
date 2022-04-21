using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Notification;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;
namespace URLS.Application.Services.Implementations
{
    public class NotificationService : BaseService<Notification>, INotificationService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public NotificationService(URLSDbContext db, IMapper mapper, IIdentityService identityService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<NotificationViewModel>> GetNotificationByIdAsync(long notifyId)
        {
            var notification = await _db.Notifications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == notifyId);
            if (notification == null)
                return Result<NotificationViewModel>.NotFound(typeof(Notification).NotFoundMessage(notifyId));

            if (!_identityService.IsAdministrator())
                if (notification.UserId != _identityService.GetUserId())
                    return Result<NotificationViewModel>.Forbiden();

            return Result<NotificationViewModel>.SuccessWithData(_mapper.Map<NotificationViewModel>(notification));
        }

        public async Task<Result<List<NotificationViewModel>>> GetUserNotificationsAsync(int userId, int offset, int count)
        {
            if (!_identityService.IsAdministrator())
                if (userId != _identityService.GetUserId())
                    return Result<List<NotificationViewModel>>.Forbiden();

            var notifications = await _db.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(offset).Take(count)
                .ToListAsync();

            return Result<List<NotificationViewModel>>.SuccessWithData(_mapper.Map<List<NotificationViewModel>>(notifications));
        }

        public async Task<Result<NotificationViewModel>> ReadNotificationAsync(long notifyId)
        {
            var notification = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == notifyId);

            if (!_identityService.IsAdministrator())
                if (notification.UserId != _identityService.GetUserId())
                    return Result<NotificationViewModel>.Forbiden();

            if (notification.IsRead)
                return Result<NotificationViewModel>.Error("Notification is already reading");

            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;
            notification.PrepareToUpdate(_identityService);

            _db.Notifications.Update(notification);
            await _db.SaveChangesAsync();

            return Result<NotificationViewModel>.SuccessWithData(_mapper.Map<NotificationViewModel>(notification));
        }

        public async Task<Result<bool>> SendNotifyToUserAsync(Notification notification, int userId)
        {
            notification.UserId = userId;
            notification.PrepareToCreate(_identityService);
            await _db.Notifications.AddAsync(notification);
            await _db.SaveChangesAsync();
            return Result<bool>.SuccessWithData(true);
        }

        public async Task<Result<bool>> SendNotifyToUsersAsync(Notification notification, IEnumerable<int> userIds)
        {
            var notifications = new List<Notification>();

            foreach (int userId in userIds)
            {
                var notify = new Notification
                {
                    Title = notification.Title,
                    Content = notification.Content,
                    ImageUrl = notification.ImageUrl,
                    IsImportant = notification.IsImportant,
                    Type = notification.Type,
                    UserId = userId
                };
                notify.PrepareToCreate();
                notifications.Add(notify);
            }
            await _db.Notifications.AddRangeAsync(notifications);
            await _db.SaveChangesAsync();
            return Result<bool>.SuccessWithData(true);
        }
    }
}