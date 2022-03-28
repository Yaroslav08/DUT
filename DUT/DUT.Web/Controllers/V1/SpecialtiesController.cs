using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Specialty;
using DUT.Constants;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class SpecialtiesController : ApiBaseController
    {
        private readonly ISpecialtyService _specialtyService;
        private readonly IPermissionService _permissionService;
        public SpecialtiesController(ISpecialtyService specialtyService, IPermissionService permissionService)
        {
            _specialtyService = specialtyService;
            _permissionService = permissionService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateSpecialty([FromBody] SpecialtyCreateModel model)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Specialties, Permissions.CanCreate))
                return JsonForbiddenResult();
            return JsonResult(await _specialtyService.CreateSpecialtyAsync(model));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSpecialty([FromBody] SpecialtyEditModel model)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Specialties, Permissions.CanEdit))
                return JsonForbiddenResult();
            return JsonResult(await _specialtyService.UpdateSpecialtyAsync(model));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSpecialties()
        {
            if (!_permissionService.HasPermission(PermissionClaims.Specialties, Permissions.CanViewAll))
                return JsonForbiddenResult();
            return JsonResult(await _specialtyService.GetAllSpecialtiesAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpecialtyById(int id)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Specialties, Permissions.CanView))
                return JsonForbiddenResult();
            return JsonResult(await _specialtyService.GetSpecialtyByIdAsync(id));
        }
    }
}