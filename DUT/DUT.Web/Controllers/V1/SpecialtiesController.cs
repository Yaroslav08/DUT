using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Specialty;
using Microsoft.AspNetCore.Mvc;
namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class SpecialtiesController : ApiBaseController
    {
        private readonly ISpecialtyService _specialtyService;
        public SpecialtiesController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateSpecialty([FromBody] SpecialtyCreateModel model)
        {
            return JsonResult(await _specialtyService.CreateSpecialtyAsync(model));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSpecialty([FromBody] SpecialtyEditModel model)
        {
            return JsonResult(await _specialtyService.UpdateSpecialtyAsync(model));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSpecialties()
        {
            return JsonResult(await _specialtyService.GetAllSpecialtiesAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpecialtyById(int id)
        {
            return JsonResult(await _specialtyService.GetSpecialtyByIdAsync(id));
        }
    }
}