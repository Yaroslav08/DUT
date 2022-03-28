using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Apps;
using DUT.Constants;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class AppsController : ApiBaseController
    {
        private readonly IAppService _appService;
        private readonly IPermissionService _permissionService;
        public AppsController(IAppService appService, IPermissionService permissionService)
        {
            _appService = appService;
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllApps()
        {
            if (!_permissionService.HasPermission(PermissionClaims.Apps, Permissions.CanView))
                return JsonForbiddenResult();
            return JsonResult(await _appService.GetAllAppsAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppById(int id)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Apps, Permissions.CanView))
                return JsonForbiddenResult();
            return JsonResult(await _appService.GetAppByIdAsync(id));
        }

        [HttpPost("{id}/new-secret")]
        public async Task<IActionResult> UpdateApp(int id)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Apps, Permissions.CanEdit))
                return JsonForbiddenResult();
            return JsonResult(await _appService.ChangeAppSecretAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateApp([FromBody] AppCreateModel appModel)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Apps, Permissions.CanCreate))
                return JsonForbiddenResult();
            return JsonResult(await _appService.CreateAppAsync(appModel));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateApp([FromBody] AppEditModel appModel)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Apps, Permissions.CanEdit))
                return JsonForbiddenResult();
            return JsonResult(await _appService.UpdateAppAsync(appModel));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApp(int id)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Apps, Permissions.CanRemove))
                return JsonForbiddenResult();
            return JsonResult(await _appService.DeleteAppAsync(id));
        }
    }
}