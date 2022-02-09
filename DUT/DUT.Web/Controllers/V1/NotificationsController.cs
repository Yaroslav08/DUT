using DUT.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class NotificationsController : ApiBaseController
    {
        private readonly INotificationService _notificationsService;
        private readonly IIdentityService _identityService;
        public NotificationsController(INotificationService notificationsService, IIdentityService identityService)
        {
            _notificationsService = notificationsService;
            _identityService = identityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications(int userId = default)
        {
            if (userId == default || userId < 0)
                userId = _identityService.GetUserId();
            return JsonResult(await _notificationsService.GetUserNotificationsAsync(userId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotification(long id)
        {
            return JsonResult(await _notificationsService.GetNotificationByIdAsync(id));
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> ReadNotification(long id)
        {
            return JsonResult(await _notificationsService.ReadNotificationAsync(id));
        }
    }
}
