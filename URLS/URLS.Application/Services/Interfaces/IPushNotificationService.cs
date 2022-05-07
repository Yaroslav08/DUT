using URLS.Application.ViewModels.Firebase;

namespace URLS.Application.Services.Interfaces
{
    public interface IPushNotificationService
    {
        Task<PushResponse> SendPushAsync(int userId, PushMessage message);
        Task<PushResponse> SendPushAsync(IEnumerable<int> userIds, PushMessage message);
    }
}