using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers
{
    public class BaseController : Controller
    {
        public IActionResult Index()
        {
            var meta = HttpContext.GetEndpoint();
            return View();
        }
    }
}
