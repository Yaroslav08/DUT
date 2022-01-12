using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Specialty;
using Microsoft.AspNetCore.Mvc;
namespace DUT.Web.Controllers
{
    public class SpecialtyController : Controller
    {
        private readonly ISpecialtyService _specialtyService;
        private readonly IFacultyService _facultyService;
        public SpecialtyController(ISpecialtyService specialtyService, IFacultyService facultyService)
        {
            _specialtyService = specialtyService;
            _facultyService = facultyService;
        }


        [HttpGet("specialty/all")]
        public async Task<IActionResult> GetAllSpecialties()
        {
            var result = await _specialtyService.GetAllSpecialtiesAsync();
            return View(result.Data);
        }

        [HttpGet("specialty/{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var result = await _specialtyService.GetSpecialtyByIdAsync(id);
            if (result.IsNotFound)
                return LocalRedirect("~/specialty/all");
            return View(result.Data);
        }

        [HttpGet("specialty/new")]
        public async Task<IActionResult> Create(string name = null)
        {
            var facultyResult = await _facultyService.GetAllFacultiesAsync();
            ViewBag.Faculties = facultyResult.Data;
            return View(new SpecialtyCreateModel
            {
                Name = name
            });
        }

        [HttpPost("specialty/new")]
        public async Task<IActionResult> Create(SpecialtyCreateModel model)
        {
            var result = await _specialtyService.CreateSpecialtyAsync(model);
            if (result.IsNotFound)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                var facultyResult = await _facultyService.GetAllFacultiesAsync();
                ViewBag.Faculties = facultyResult.Data;
                return View(model);
            }
            return LocalRedirect("~/specialty/all");
        }

        [HttpGet("specialty/update")]
        public async Task<IActionResult> Update(int id)
        {
            var facultyResult = await _facultyService.GetAllFacultiesAsync();
            ViewBag.Faculties = facultyResult.Data;
            var specialtyToEdit = await _specialtyService.GetSpecialtyByIdAsync(id);
            if (specialtyToEdit.IsNotFound)
                return LocalRedirect("~/specialty/all");
            return View(new SpecialtyEditModel(specialtyToEdit.Data));
        }

        [HttpPost("specialty/update")]
        public async Task<IActionResult> Update(SpecialtyEditModel model)
        {
            var result = await _specialtyService.UpdateSpecialtyAsync(model);
            if (result.IsNotFound)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                var facultyResult = await _facultyService.GetAllFacultiesAsync();
                ViewBag.Faculties = facultyResult.Data;
                return View(model);
            }
            return LocalRedirect("~/specialty/all");
        }
    }
}