using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Specialty;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers.Api.V1
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
            var result = await _specialtyService.CreateSpecialtyAsync(model);
            if (result.IsNotFound)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSpecialty([FromBody] SpecialtyEditModel model)
        {
            var result = await _specialtyService.UpdateSpecialtyAsync(model);
            if (result.IsNotFound)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSpecialties()
        {
            return Ok(await _specialtyService.GetAllSpecialtiesAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpecialtyById(int id)
        {
            var result = await _specialtyService.GetSpecialtyByIdAsync(id);
            if (result.IsNotFound)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }
    }
}