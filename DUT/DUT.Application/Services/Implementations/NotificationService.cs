using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Notification;
using DUT.Application.ViewModels.Session;
using DUT.Constants;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DUT.Application.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public NotificationService(DUTDbContext db, IMapper mapper, IIdentityService identityService)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<NotificationViewModel>> GetNotificationByIdAsync(long notifyId)
        {
            var notification = await _db.Notifications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == notifyId);
            if (notification == null)
                return Result<NotificationViewModel>.NotFound("Notification not found");
            if (notification.UserId != _identityService.GetUserId() || _identityService.GetRoles().Contains(Roles.Admin))
                return Result<NotificationViewModel>.Error("Access denited");
            return Result<NotificationViewModel>.SuccessWithData(_mapper.Map<NotificationViewModel>(notification));
        }

        public async Task<Result<List<NotificationViewModel>>> GetUserNotificationsAsync(int userId)
        {
            if (userId != _identityService.GetUserId() || _identityService.GetRoles().Contains(Roles.Admin))
                return Result<List<NotificationViewModel>>.Error("Access denited");

            var notifications = await _db.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Result<List<NotificationViewModel>>.SuccessWithData(_mapper.Map<List<NotificationViewModel>>(notifications));
        }

        public async Task<Result<NotificationViewModel>> ReadNotificationAsync(long notifyId)
        {
            var notification = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == notifyId);
            if (notification.UserId != _identityService.GetUserId() || _identityService.GetRoles().Contains(Roles.Admin))
                return Result<NotificationViewModel>.Error("Access denited");

            if (notification.IsRead)
                return Result<NotificationViewModel>.Error("Notification is already reading");

            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;
            notification.PrepareToUpdate(_identityService);

            _db.Notifications.Update(notification);
            await _db.SaveChangesAsync();

            return Result<NotificationViewModel>.SuccessWithData(_mapper.Map<NotificationViewModel>(notification));
        }
    }
}