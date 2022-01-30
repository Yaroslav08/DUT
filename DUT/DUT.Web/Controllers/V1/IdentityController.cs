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
        public IdentityController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet("claims")]
        [Authorize]
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
        public async Task<IActionResult> Login([FromBody] LoginCreateModel model)
        {
            return JsonResult(await _authenticationService.LoginByPasswordAsync(model));
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] RegisterViewModel model)
        {
            return JsonResult(await _authenticationService.RegisterAsync(model));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            return JsonResult(await _authenticationService.LogoutAsync());
        }
    }
}