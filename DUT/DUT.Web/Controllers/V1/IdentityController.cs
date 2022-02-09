using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Identity;
using DUT.Application.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class IdentityController : ApiBaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ISessionService _sessionService;
        private readonly IIdentityService _identityService;
        public IdentityController(IAuthenticationService authenticationService, ISessionService sessionService, IIdentityService identityService)
        {
            _authenticationService = authenticationService;
            _sessionService = sessionService;
            _identityService = identityService;
        }

        [HttpGet("claims")]
        public IActionResult GetClaims()
        {
            var data = User.Claims.Select(x => new
            {
                Type = x.Type,
                Value = x.Value
            }).ToList();
            return Ok(data);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCreateModel model)
        {
            return JsonResult(await _authenticationService.LoginByPasswordAsync(model));
        }

        [HttpPost("registration")]
        [AllowAnonymous]
        public async Task<IActionResult> Registration([FromBody] RegisterViewModel model)
        {
            return JsonResult(await _authenticationService.RegisterAsync(model));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            return JsonResult(await _authenticationService.LogoutAsync());
        }

        [HttpGet("sessions")]
        public async Task<IActionResult> GetActiveSessions(int userId = default)
        {
            if (userId == default || userId < 0)
                userId = _identityService.GetUserId();
            return JsonResult(await _sessionService.GetActiveSessionsByUserIdAsync(userId));
        }

        [HttpGet("sessions/{id}")]
        public async Task<IActionResult> GetSessionById(Guid id)
        {
            return JsonResult(await _sessionService.GetSessionByIdAsync(id));
        }

        [HttpGet("sessions/unactive")]
        public async Task<IActionResult> GetAllSessions(int userId = default)
        {
            if (userId == default || userId < 0)
                userId = _identityService.GetUserId();
            return JsonResult(await _sessionService.GetAllSessionsByUserIdAsync(userId));
        }

        [HttpPost("config")]
        public async Task<IActionResult> ConfigUser([FromBody] BlockUserModel model)
        {
            return JsonResult(await _authenticationService.BlockUserConfigAsync(model));
        }
    }
}