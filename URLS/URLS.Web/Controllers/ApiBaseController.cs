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
            if (result.IsSuccess)
                return Ok(APIResponse.OkResponse(result.Data));
            if (result.IsNotFound)
                return NotFound(APIResponse.NotFoundResponse(result.ErrorMessage));
            if (result.IsError)
                return BadRequest(APIResponse.BadRequestResponse(result.ErrorMessage));
            if (result.IsForbid)
                return JsonForbiddenResult();
            return JsonInternalServerErrorResult();
        }

        [NonAction]
        public IActionResult JsonForbiddenResult()
        {
            HttpContext.Response.StatusCode = 403;
            return Json(APIResponse.ForbiddenResposne());
        }

        [NonAction]
        public IActionResult JsonInternalServerErrorResult()
        {
            HttpContext.Response.StatusCode = 500;
            return Json(APIResponse.InternalServerError(HttpContext.TraceIdentifier));
        }

        [NonAction]
        public IActionResult CreatedResult(object data)
        {
            HttpContext.Response.StatusCode = 201;
            return Json(APIResponse.OkResponse(data));
        }
    }
}