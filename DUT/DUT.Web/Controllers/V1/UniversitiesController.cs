using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.University;
using DUT.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class UniversitiesController : ApiBaseController
    {
        private readonly IUniversityService _universityService;
        private readonly IPermissionService _permissionService;
        public UniversitiesController(IUniversityService universityService, IPermissionService permissionService)
        {
            _universityService = universityService;
            _permissionService = permissionService;
        }



        [HttpGet]
        public async Task<IActionResult> GetUniversity()
        {
            if (!await _permissionService.HasPermissionAsync(PermissionClaims.University, Permissions.CanView))
            {
                return JsonForbiddenResult();
            }
            return JsonResult(await _universityService.GetUniversityAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUniversity([FromBody] UniversityCreateModel model)
        {
            if (!await _permissionService.HasPermissionAsync(PermissionClaims.University, Permissions.CanCreate))
            {
                return JsonForbiddenResult();
            }
            return JsonResult(await _universityService.CreateUniversityAsync(model));
        }

        [HttpPut]
        public async Task<IActionResult> EditUniversity([FromBody] UniversityEditModel model)
        {
            if (!await _permissionService.HasPermissionAsync(PermissionClaims.University, Permissions.CanEdit))
            {
                return JsonForbiddenResult();
            }
            return JsonResult(await _universityService.UpdateUniversityAsync(model));
        }
    }
}