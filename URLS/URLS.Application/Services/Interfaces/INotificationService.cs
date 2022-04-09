using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Notification;
using URLS.Domain.Models;

namespace URLS.Application.Services.Interfaces
{
    public interface INotificationService : IBaseService<Notification>
    {
        Task<Result<List<NotificationViewModel>>> GetUserNotificationsAsync(int userId);
        Task<Result<NotificationViewModel>> GetNotificationByIdAsync(long notifyId);
        Task<Result<NotificationViewModel>> ReadNotificationAsync(long notifyId);
        Task<Result<bool>> SendNotifyByUserIdsAsync(Notification notification, IEnumerable<int> userIds);
    }
}