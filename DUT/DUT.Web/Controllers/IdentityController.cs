using DUT.Application.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers
{
    [Route("Identity")]
    public class IdentityController : BaseController
    {
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public IActionResult LoginUser(LoginViewModel loginModel)
        {
            if(!ModelState.IsValid)
            {
                return View(loginModel);
            }
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
            if (!ModelState.IsValid)
            {
                return View(registerModel);
            }
            return LocalRedirect("~/");
        }


        [HttpGet("logout")]
        public IActionResult Logout()
        {
            return LocalRedirect("~/");
        }
    }
}
