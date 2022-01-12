using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.University;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers.Api.V1
{
    [ApiVersion("1.0")]
    public class UniversitiesController : ApiBaseController
    {
        private readonly IUniversityService _universityService;
        public UniversitiesController(IUniversityService universityService)
        {
            _universityService = universityService;
        }



        [HttpGet]
        public async Task<IActionResult> GetUniversity()
        {
            return JsonResult(await _universityService.GetUniversityAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUniversity([FromBody] UniversityCreateModel model)
        {
            return JsonResult(await _universityService.CreateUniversityAsync(model));
        }

        [HttpPut]
        public async Task<IActionResult> EditUniversity([FromBody] UniversityEditModel model)
        {
            return JsonResult(await _universityService.UpdateUniversityAsync(model));
        }
    }
}