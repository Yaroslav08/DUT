using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Notification;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface INotificationService : IBaseService<Notification>
    {
        Task<Result<List<NotificationViewModel>>> GetUserNotificationsAsync(int userId);
        Task<Result<NotificationViewModel>> GetNotificationByIdAsync(long notifyId);
        Task<Result<NotificationViewModel>> ReadNotificationAsync(long notifyId);
        Task<Result<bool>> SendNotifyByUserIdsAsync(Notification notification, IEnumerable<int> userIds);
    }
}