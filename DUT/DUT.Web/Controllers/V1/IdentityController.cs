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

        #region Identity

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

        [HttpPost("config")]
        public async Task<IActionResult> ConfigUser([FromBody] BlockUserModel model)
        {
            return JsonResult(await _authenticationService.BlockUserConfigAsync(model));
        }

        #endregion

        #region Sessions

        [HttpGet("sessions")]
        public async Task<IActionResult> GetActiveSessions(int userId = default, int q = 0, int offset = 0, int limit = 20)
        {
            if (userId == default || userId < 1)
                userId = _identityService.GetUserId();
            return JsonResult(await _sessionService.GetAllSessionsByUserIdAsync(userId, q, offset, limit));
        }

        [HttpGet("sessions/{id}")]
        public async Task<IActionResult> GetSessionById(Guid id)
        {
            return JsonResult(await _sessionService.GetSessionByIdAsync(id));
        }

        [HttpDelete("sessions")]
        public async Task<IActionResult> CloseSessions(int userId = default, bool withCurrent = false)
        {
            if (userId == default || userId < 1)
                userId = _identityService.GetUserId();
            return JsonResult(await _sessionService.CloseAllSessionsAsync(userId, withCurrent));
        }

        [HttpDelete("sessions/{id}")]
        public async Task<IActionResult> CloseSessionById(Guid sessionId)
        {
            return JsonResult(await _sessionService.CloseSessionByIdAsync(sessionId));
        }

        #endregion
    }
}