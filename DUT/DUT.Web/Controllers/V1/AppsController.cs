using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Apps;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class AppsController : ApiBaseController
    {
        private readonly IAppService _appService;
        public AppsController(IAppService appService)
        {
            _appService = appService;
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

        [HttpPost("{id}/new-secret")]
        public async Task<IActionResult> UpdateApp(int id)
        {
            return JsonResult(await _appService.ChangeAppSecretAsync(id));
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApp(int id)
        {
            return JsonResult(await _appService.DeleteAppAsync(id));
        }
    }
}
