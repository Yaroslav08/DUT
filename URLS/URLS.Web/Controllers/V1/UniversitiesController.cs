using Microsoft.AspNetCore.Mvc;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.University;
using URLS.Constants;
using URLS.Web.Filters;

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
        [PermissionFilter(PermissionClaims.University, Permissions.CanView)]
        public async Task<IActionResult> GetUniversity()
        {
            return JsonResult(await _universityService.GetUniversityAsync());
        }

        [HttpGet("faculties")]
        [PermissionFilter(PermissionClaims.University, Permissions.CanViewAll)]
        public async Task<IActionResult> GetUniversityFaculties()
        {
            return JsonResult(await _faultyService.GetFacultiesByUniversityIdAsync(1));
        }

        [HttpPost]
        [PermissionFilter(PermissionClaims.University, Permissions.CanCreate)]
        public async Task<IActionResult> CreateUniversity([FromBody] UniversityCreateModel model)
        {
            return JsonResult(await _universityService.CreateUniversityAsync(model));
        }

        [HttpPut]
        [PermissionFilter(PermissionClaims.University, Permissions.CanEdit)]
        public async Task<IActionResult> EditUniversity([FromBody] UniversityEditModel model)
        {
            return JsonResult(await _universityService.UpdateUniversityAsync(model));
        }
    }
}