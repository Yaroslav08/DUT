using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Apps;
using URLS.Constants;
using Microsoft.AspNetCore.Mvc;

namespace URLS.Web.Controllers.V1
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
            return JsonResult(await _appService.GetAllAppsAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppById(int id)
        {
            return JsonResult(await _appService.GetAppByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateApp([FromBody] AppCreateModel appModel)
        {
            return JsonResult(await _appService.CreateAppAsync(appModel));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateApp([FromBody] AppEditModel appModel)
        {
            return JsonResult(await _appService.UpdateAppAsync(appModel));
        }

        [HttpPost("{id}/new-secret")]
        public async Task<IActionResult> UpdateApp(int id)
        {
            return JsonResult(await _appService.ChangeAppSecretAsync(id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApp(int id)
        {
            return JsonResult(await _appService.DeleteAppAsync(id));
        }
    }
}