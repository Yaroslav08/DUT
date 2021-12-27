using Microsoft.AspNetCore.Mvc;
namespace DUT.Web.Controllers.Api
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ApiBaseController : BaseController
    {

    }
}