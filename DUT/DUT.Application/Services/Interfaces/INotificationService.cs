using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Notification;

namespace DUT.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task<Result<List<NotificationViewModel>>> GetUserNotificationsAsync(int userId);
        Task<Result<NotificationViewModel>> GetNotificationByIdAsync(long notifyId);
        Task<Result<NotificationViewModel>> ReadNotificationAsync(long notifyId);
    }
}