using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.RoleClaim;
using DUT.Constants;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class PermissionsController : ApiBaseController
    {
        private readonly IIdentityService _identityService;
        private readonly IRoleClaimsService _roleClaimsService;
        private readonly IPermissionService _permissionService;
        public PermissionsController(IIdentityService identityService, IRoleClaimsService roleClaimsService, IPermissionService permissionService)
        {
            _identityService = identityService;
            _roleClaimsService = roleClaimsService;
            _permissionService = permissionService;
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            if (!_permissionService.HasPermission(PermissionClaims.Permissions, Permissions.All))
                return JsonForbiddenResult();
            return JsonResult(await _roleClaimsService.GetAllRolesAsync());
        }

        [HttpGet("roles/{id}")]
        public async Task<IActionResult> GetRoleById(int id, bool withClaims = false)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Permissions, Permissions.All))
                return JsonForbiddenResult();
            return JsonResult(await _roleClaimsService.GetRoleByIdAsync(id, withClaims));
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateModel model)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Permissions, Permissions.All))
                return JsonForbiddenResult();
            return JsonResult(await _roleClaimsService.CreateRoleAsync(model));
        }

        [HttpPut("roles/{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleEditModel model)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Permissions, Permissions.All))
                return JsonForbiddenResult();
            model.Id = id;
            return JsonResult(await _roleClaimsService.UpdateRoleAsync(model));
        }

        [HttpDelete("roles/{id}")]
        public async Task<IActionResult> RemoveRole(int id)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Permissions, Permissions.All))
                return JsonForbiddenResult();
            return JsonResult(await _roleClaimsService.RemoveRoleAsync(id));
        }

        [HttpGet("claims")]
        public async Task<IActionResult> GetAllClaims()
        {
            if (!_permissionService.HasPermission(PermissionClaims.Permissions, Permissions.All))
                return JsonForbiddenResult();
            return JsonResult(await _roleClaimsService.GetClaimsAsync());
        }

        [HttpPut("claims/{id}")]
        public async Task<IActionResult> UpdateClaim(int id, [FromBody] ClaimEditModel model)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Permissions, Permissions.All))
                return JsonForbiddenResult();
            model.Id = id;
            return JsonResult(await _roleClaimsService.UpdateClaimAsync(model));
        }
    }
}
