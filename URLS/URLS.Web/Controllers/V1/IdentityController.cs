using Microsoft.AspNetCore.Mvc;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.RoleClaim;
namespace URLS.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class IdentityController : ApiBaseController
    {
        private readonly IIdentityService _identityService;
        private readonly IRoleService _roleService;
        private readonly IClaimService _claimService;
        private readonly IPermissionService _permissionService;
        public IdentityController(IIdentityService identityService, IRoleService roleService, IClaimService claimService, IPermissionService permissionService)
        {
            _identityService = identityService;
            _roleService = roleService;
            _claimService = claimService;
            _permissionService = permissionService;
        }

        #region Roles

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            return JsonResult(await _roleService.GetAllRolesAsync());
        }

        [HttpGet("roles/{id}")]
        public async Task<IActionResult> GetRoleById(int id, bool withClaims = false)
        {
            return JsonResult(await _roleService.GetRoleByIdAsync(id, withClaims));
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateModel model)
        {
            return JsonResult(await _roleService.CreateRoleAsync(model));
        }

        [HttpPut("roles/{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleEditModel model)
        {
            model.Id = id;
            return JsonResult(await _roleService.UpdateRoleAsync(model));
        }

        [HttpDelete("roles/{id}")]
        public async Task<IActionResult> RemoveRole(int id)
        {
            return JsonResult(await _roleService.RemoveRoleAsync(id));
        }

        #endregion

        #region Claims

        [HttpGet("claims")]
        public async Task<IActionResult> GetAllClaims()
        {
            return JsonResult(await _claimService.GetAllClaimsAsync());
        }

        [HttpPut("claims/{id}")]
        public async Task<IActionResult> UpdateClaim(int id, [FromBody] ClaimEditModel model)
        {
            model.Id = id;
            return JsonResult(await _claimService.UpdateClaimAsync(model));
        }

        #endregion
    }
}