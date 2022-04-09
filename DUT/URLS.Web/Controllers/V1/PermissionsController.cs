using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.RoleClaim;
using URLS.Constants;
using Microsoft.AspNetCore.Mvc;
namespace URLS.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class PermissionsController : ApiBaseController
    {
        private readonly IIdentityService _identityService;
        private readonly IRoleService _roleService;
        private readonly IClaimService _claimService;
        private readonly IPermissionService _permissionService;
        public PermissionsController(IIdentityService identityService, IRoleService roleService, IClaimService claimService, IPermissionService permissionService)
        {
            _identityService = identityService;
            _roleService = roleService;
            _claimService = claimService;
            _permissionService = permissionService;
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            if (!_permissionService.HasPermission(PermissionClaims.Permissions, Permissions.All))
                return JsonForbiddenResult();
            return JsonResult(await _roleService.GetAllRolesAsync());
        }

        [HttpGet("roles/{id}")]
        public async Task<IActionResult> GetRoleById(int id, bool withClaims = false)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Permissions, Permissions.All))
                return JsonForbiddenResult();
            return JsonResult(await _roleService.GetRoleByIdAsync(id, withClaims));
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateModel model)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Permissions, Permissions.All))
                return JsonForbiddenResult();
            return JsonResult(await _roleService.CreateRoleAsync(model));
        }

        [HttpPut("roles/{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleEditModel model)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Permissions, Permissions.All))
                return JsonForbiddenResult();
            model.Id = id;
            return JsonResult(await _roleService.UpdateRoleAsync(model));
        }

        [HttpDelete("roles/{id}")]
        public async Task<IActionResult> RemoveRole(int id)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Permissions, Permissions.All))
                return JsonForbiddenResult();
            return JsonResult(await _roleService.RemoveRoleAsync(id));
        }

        [HttpGet("claims")]
        public async Task<IActionResult> GetAllClaims()
        {
            if (!_permissionService.HasPermission(PermissionClaims.Permissions, Permissions.All))
                return JsonForbiddenResult();
            return JsonResult(await _claimService.GetAllClaimsAsync());
        }

        [HttpPut("claims/{id}")]
        public async Task<IActionResult> UpdateClaim(int id, [FromBody] ClaimEditModel model)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Permissions, Permissions.All))
                return JsonForbiddenResult();
            model.Id = id;
            return JsonResult(await _claimService.UpdateClaimAsync(model));
        }
    }
}