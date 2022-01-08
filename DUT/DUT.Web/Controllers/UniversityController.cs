using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.University;
using Microsoft.AspNetCore.Mvc;
namespace DUT.Web.Controllers
{
    public class UniversityController : Controller
    {
        private readonly IUniversityService _universityService;
        public UniversityController(IUniversityService universityService)
        {
            _universityService = universityService;
        }

        [HttpGet("university")]
        public async Task<IActionResult> GetDetails()
        {
            var currentUniversity = await _universityService.GetUniversityAsync();
            if (currentUniversity.IsNotFound)
                return LocalRedirect("~/university/new");
            return View(currentUniversity.Data);
        }

        [HttpGet("university/new")]
        public IActionResult NewUniversity(string name)
        {
            return View(new UniversityCreateModel
            {
                Name = name
            });
        }

        [HttpPost("university/new")]
        public async Task<IActionResult> CreateUniversity(UniversityCreateModel model)
        {
            var result = await _universityService.CreateUniversityAsync(model);
            if (result.IsSuccess)
                return LocalRedirect("~/university");
            ModelState.AddModelError("", result.ErrorMessage);
            return View("NewUniversity",model);
        }


        [HttpGet("university/edit")]
        public async Task<IActionResult> EditUniversity()
        {
            var currentUniversity = await _universityService.GetUniversityAsync();
            if (currentUniversity.IsNotFound)
                return LocalRedirect("~/university/new");
            return View(new UniversityEditModel(currentUniversity.Data));
        }

        [HttpPost("university/edit")]
        public async Task<IActionResult> EditUniversity(UniversityEditModel model)
        {
            var result = await _universityService.UpdateUniversityAsync(model);
            if (result.IsSuccess)
                return LocalRedirect("~/university");
            ModelState.AddModelError("", result.ErrorMessage);
            return View("EditUniversity", model);
        }
    }
}