using Microsoft.AspNetCore.Mvc;
namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class IdentityController : ApiBaseController
    {

        [HttpPost("login")]
        public IActionResult Login()
        {
            return Ok();
        }

        [HttpPost("registration")]
        public IActionResult Registration()
        {
            return Ok();
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok();
        }
    }
}