using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers.Api.V1
{
    [ApiVersion("1.0")]
    public class RolesController : ApiBaseController
    {
        private readonly IRoleService _roleService;
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }


        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            return JsonResult(Result<List<Role>>.SuccessWithData(await _roleService.GetAllRolesAsync()));
        }

    }
}
