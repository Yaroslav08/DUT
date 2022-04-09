using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Setting;
using URLS.Constants;
using Microsoft.AspNetCore.Mvc;
namespace URLS.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class SettingsController : ApiBaseController
    {
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        public SettingsController(IPermissionService permissionService, ISettingService settingService)
        {
            _permissionService = permissionService;
            _settingService = settingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSetting()
        {
            if (!_permissionService.HasPermission(PermissionClaims.Settings, Permissions.CanView))
                return JsonForbiddenResult();
            return JsonResult(await _settingService.GetRootSettingAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateSetting([FromBody] SettingCreateModel model)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Settings, Permissions.CanCreate))
                return JsonForbiddenResult();
            return JsonResult(await _settingService.CreateSettingAsync(model));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSetting([FromBody] SettingEditModel model)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Settings, Permissions.CanEdit))
                return JsonForbiddenResult();
            return JsonResult(await _settingService.UpdateSettingAsync(model));
        }
    }
}