using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Faculty;
using URLS.Constants;
using Microsoft.AspNetCore.Mvc;
namespace URLS.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class FacultiesController : ApiBaseController
    {
        private readonly IFacultyService _facultyService;
        private readonly ISpecialtyService _specialtyService;
        private readonly IPermissionService _permissionService;
        public FacultiesController(IFacultyService facultyService, IPermissionService permissionService, ISpecialtyService specialtyService)
        {
            _facultyService = facultyService;
            _permissionService = permissionService;
            _specialtyService = specialtyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFaculties()
        {
            if (!_permissionService.HasPermission(PermissionClaims.Faculties, Permissions.CanViewAll))
                return JsonForbiddenResult();
            return JsonResult(await _facultyService.GetAllFacultiesAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFacultyById(int id)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Faculties, Permissions.CanView))
                return JsonForbiddenResult();
            return JsonResult(await _facultyService.GetFacultyByIdAsync(id));
        }

        [HttpGet("{facultyId}/specialties")]
        public async Task<IActionResult> GetFacultySpecialties(int facultyId)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Specialties, Permissions.CanViewAll))
                return JsonForbiddenResult();
            return JsonResult(await _specialtyService.GetSpecialtiesByFacultyIdAsync(facultyId));
        }

        [HttpPost]
        public async Task<IActionResult> CreateFaculty([FromBody] FacultyCreateModel model)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Faculties, Permissions.CanCreate))
                return JsonForbiddenResult();
            return JsonResult(await _facultyService.CreateFacultyAsync(model));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFaculty([FromBody] FacultyEditModel model)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Faculties, Permissions.CanEdit))
                return JsonForbiddenResult();
            return JsonResult(await _facultyService.UpdateFacultyAsync(model));
        }
    }
}