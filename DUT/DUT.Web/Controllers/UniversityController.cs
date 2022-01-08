using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.University;
using Microsoft.AspNetCore.Mvc;
namespace DUT.Web.Controllers
{
    public class UniversityController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IUniversityService _universityService;
        public UniversityController(IIdentityService identityService, IUniversityService universityService)
        {
            _identityService = identityService;
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
    }
}