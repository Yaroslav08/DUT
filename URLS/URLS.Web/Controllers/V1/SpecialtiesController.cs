using Microsoft.AspNetCore.Mvc;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Specialty;
using URLS.Constants;
using URLS.Web.Filters;

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
        [PermissionFilter(PermissionClaims.Specialties, Permissions.CanViewAll)]
        public async Task<IActionResult> GetAllSpecialties()
        {
            return JsonResult(await _specialtyService.GetAllSpecialtiesAsync());
        }

        [HttpGet("{id}")]
        [PermissionFilter(PermissionClaims.Specialties, Permissions.CanView)]
        public async Task<IActionResult> GetSpecialtyById(int id)
        {
            return JsonResult(await _specialtyService.GetSpecialtyByIdAsync(id));
        }

        [HttpGet("{id}/groups")]
        [PermissionFilter(PermissionClaims.Specialties, Permissions.CanViewAllGroups)]
        public async Task<IActionResult> GetSpecialtyGroups(int id)
        {
            return JsonResult(await _groupService.GetGroupsBySpecialtyIdAsync(id));
        }

        [HttpPost]
        [PermissionFilter(PermissionClaims.Specialties, Permissions.CanCreate)]
        public async Task<IActionResult> CreateSpecialty([FromBody] SpecialtyCreateModel model)
        {
            return JsonResult(await _specialtyService.CreateSpecialtyAsync(model));
        }

        [HttpPut]
        [PermissionFilter(PermissionClaims.Specialties, Permissions.CanEdit)]
        public async Task<IActionResult> UpdateSpecialty([FromBody] SpecialtyEditModel model)
        {
            return JsonResult(await _specialtyService.UpdateSpecialtyAsync(model));
        }
    }
}