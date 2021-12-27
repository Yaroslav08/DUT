using DUT.Application.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers
{
    [Route("Identity")]
    public class IdentityController : Controller
    {
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public IActionResult LoginUser(LoginViewModel loginModel)
        {
            return LocalRedirect("~/");
        }

        [HttpGet("Registration")]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost("Registration")]
        public IActionResult RegistrationUser(RegisterViewModel registerModel)
        {
            return LocalRedirect("~/");
        }
    }
}
