using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.University;
using URLS.Constants;
using Microsoft.AspNetCore.Mvc;
namespace URLS.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class UniversitiesController : ApiBaseController
    {
        private readonly IUniversityService _universityService;
        private readonly IFacultyService _faultyService;
        private readonly IPermissionService _permissionService;
        public UniversitiesController(IUniversityService universityService, IPermissionService permissionService, IFacultyService faultyService)
        {
            _universityService = universityService;
            _permissionService = permissionService;
            _faultyService = faultyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUniversity()
        {
            if (!_permissionService.HasPermission(PermissionClaims.University, Permissions.CanView))
            {
                return JsonForbiddenResult();
            }
            return JsonResult(await _universityService.GetUniversityAsync());
        }

        [HttpGet("faculties")]
        public async Task<IActionResult> GetUniversityFaculties()
        {
            return JsonResult(await _faultyService.GetFacultiesByUniversityIdAsync(1));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUniversity([FromBody] UniversityCreateModel model)
        {
            if (!_permissionService.HasPermission(PermissionClaims.University, Permissions.CanCreate))
            {
                return JsonForbiddenResult();
            }
            return JsonResult(await _universityService.CreateUniversityAsync(model));
        }

        [HttpPut]
        public async Task<IActionResult> EditUniversity([FromBody] UniversityEditModel model)
        {
            if (!_permissionService.HasPermission(PermissionClaims.University, Permissions.CanEdit))
            {
                return JsonForbiddenResult();
            }
            return JsonResult(await _universityService.UpdateUniversityAsync(model));
        }
    }
}