using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Specialty;
using URLS.Constants;
using Microsoft.AspNetCore.Mvc;
namespace URLS.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class SpecialtiesController : ApiBaseController
    {
        private readonly ISpecialtyService _specialtyService;
        private readonly IGroupService _groupService;
        private readonly IPermissionService _permissionService;
        public SpecialtiesController(ISpecialtyService specialtyService, IPermissionService permissionService, IGroupService groupService)
        {
            _specialtyService = specialtyService;
            _permissionService = permissionService;
            _groupService = groupService;
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

        [HttpGet("{id}/groups")]
        public async Task<IActionResult> GetSpecialtyGroups(int id)
        {
            return JsonResult(await _groupService.GetGroupsBySpecialtyIdAsync(id));
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
    }
}