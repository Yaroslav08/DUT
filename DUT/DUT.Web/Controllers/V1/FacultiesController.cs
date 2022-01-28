using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Faculty;
using Microsoft.AspNetCore.Mvc;
namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class FacultiesController : ApiBaseController
    {
        private readonly IFacultyService _facultyService;
        public FacultiesController(IFacultyService facultyService)
        {
            _facultyService = facultyService;
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
            return JsonResult(await _facultyService.GetSpecialtiesByFacultyIdAsync(facultyId));
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