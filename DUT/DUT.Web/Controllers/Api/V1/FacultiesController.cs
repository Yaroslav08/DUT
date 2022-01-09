using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Faculty;
using Microsoft.AspNetCore.Mvc;
namespace DUT.Web.Controllers.Api.V1
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
            var result = await _facultyService.GetAllFacultiesAsync();
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFaculty([FromBody] FacultyCreateModel model)
        {
            var result = await _facultyService.CreateFacultyAsync(model);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFaculty([FromBody] FacultyEditModel model)
        {
            var result = await _facultyService.UpdateFacultyAsync(model);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

    }
}