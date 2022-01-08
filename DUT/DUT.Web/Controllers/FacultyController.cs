using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Faculty;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers
{
    public class FacultyController : Controller
    {
        private readonly IFacultyService _facultyService;
        public FacultyController(IFacultyService facultyService)
        {
            _facultyService = facultyService;
        }


        [HttpGet("faculty/all")]
        public async Task<IActionResult> GetAllFaculties()
        {
            var faculties = await _facultyService.GetAllFacultiesAsync();
            if (faculties.Data is null || faculties.Data.Count == 0)
                return LocalRedirect("~/faculty/new");
            return View(faculties.Data);
        }

        [HttpGet("faculty/{id}")]
        public async Task<IActionResult> GetFaculty(int id)
        {
            var faculty = await _facultyService.GetFacultyByIdAsync(id);
            if (faculty.IsNotFound)
                return LocalRedirect("~/faculty/new");
            return View(faculty.Data);
        }

        [HttpGet("faculty/new")]
        public IActionResult CreateFaculty(string name)
        {
            return View(new FacultyCreateModel
            {
                Name = name
            });
        }

        [HttpPost("faculty/new")]
        public async Task<IActionResult> CreateFaculty(FacultyCreateModel faculty)
        {
            var result = await _facultyService.CreateFacultyAsync(faculty);
            if (result.IsSuccess)
                return LocalRedirect("~/faculty/all");
            ModelState.AddModelError("", result.ErrorMessage);
            return View(faculty);
        }


        [HttpGet("faculty/edit")]
        public async Task<IActionResult> EditFaculty(int id)
        {
            var result = await _facultyService.GetFacultyByIdAsync(id);
            if (result.IsNotFound)
                return LocalRedirect("~/faculty/new");
            return View(new FacultyEditModel(result.Data));
        }

        [HttpPost("faculty/edit")]
        public async Task<IActionResult> EditFaculty(FacultyEditModel model)
        {
            var result = await _facultyService.UpdateFacultyAsync(model);
            if (result.IsSuccess)
                return LocalRedirect($"faculty/{model.Id}");
            ModelState.AddModelError("", result.ErrorMessage);
            return View(model);
        }
    }
}
