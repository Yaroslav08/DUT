using DUT.Application.ViewModels;
using DUT.Constants.APIResponse;
using Microsoft.AspNetCore.Mvc;
namespace DUT.Web.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ApiBaseController : Controller
    {
        public IActionResult JsonResult<T>(Result<T> result)
        {
            if (result.IsNotFound)
                return NotFound(APIResponse.NotFoundResponse(result.ErrorMessage));
            if (result.IsError)
                return BadRequest(APIResponse.BadRequestResponse(result.ErrorMessage));
            if(result.IsSuccess)
                return Ok(APIResponse.OkResponse(result.Data));
            HttpContext.Response.StatusCode = 500;
            return Json(APIResponse.InternalServerError(HttpContext.TraceIdentifier));
        }
    }
}