using URLS.Application.ViewModels;
using URLS.Constants.APIResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace URLS.Web.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class ApiBaseController : Controller
    {
        [NonAction]
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

        [NonAction]
        public IActionResult JsonForbiddenResult()
        {
            HttpContext.Response.StatusCode = 403;
            return Json(APIResponse.ForbiddenResposne());
        }
    }
}