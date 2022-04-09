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
            return JsonResult(await _facultyService.GetAllFacultiesAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFacultyById(int id)
        {
            return JsonResult(await _facultyService.GetFacultyByIdAsync(id));
        }

        [HttpGet("{facultyId}/specialties")]
        public async Task<IActionResult> GetFacultySpecialties(int facultyId)
        {
            return JsonResult(await _specialtyService.GetSpecialtiesByFacultyIdAsync(facultyId));
        }

        [HttpPost]
        public async Task<IActionResult> CreateFaculty([FromBody] FacultyCreateModel model)
        {
            return JsonResult(await _facultyService.CreateFacultyAsync(model));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFaculty([FromBody] FacultyEditModel model)
        {
            return JsonResult(await _facultyService.UpdateFacultyAsync(model));
        }
    }
}