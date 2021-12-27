using Microsoft.AspNetCore.Mvc;
namespace DUT.Web.Controllers
{
    public class NewAppController : Controller
    {
        [HttpGet("new")]
        public IActionResult Index()
        {
            return LocalRedirect("~/");
        }
    }
}