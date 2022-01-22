using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Apps;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers
{
    public class AppController : Controller
    {
        private readonly IAppService _appService;
        public AppController(IAppService appService)
        {
            _appService = appService;
        }



        [HttpGet("app/all")]
        public async Task<IActionResult> AllApps()
        {
            var appsResult = await _appService.GetAllAppsAsync();
            if (appsResult.IsNotFound)
                return LocalRedirect("~/app/new");
            return View(appsResult.Data);
        }


        [HttpGet("app/new")]
        public IActionResult CreateApp()
        {
            return View();
        }

        [HttpPost("app/new")]
        public async Task<IActionResult> CreateApp(AppCreateModel model)
        {
            var appResult = await _appService.CreateAppAsync(model);
            if (appResult.IsSuccess)
                return View("AppDetails", appResult.Data);
            return View(model);
        }

        [HttpGet("app/edit")]
        public async Task<IActionResult> EditApp(int id)
        {
            var appResult = await _appService.GetAppByIdAsync(id);
            if (appResult.IsError)
            {
                return LocalRedirect("~/app/all");
            }
            return View(new AppEditModel(appResult.Data));
        }

        [HttpPost("app/edit")]
        public async Task<IActionResult> EditApp(AppEditModel model)
        {
            var appResult = await _appService.UpdateAppAsync(model);
            if (appResult.IsSuccess)
                return View("AppDetails", appResult.Data);
            return View(model);
        }


        [HttpGet("app/{id}")]
        public async Task<IActionResult> AppDetails(int id)
        {
            var appResult = await _appService.GetAppByIdAsync(id);
            if (appResult.IsError)
                return LocalRedirect("~/app/all");
            return View(appResult.Data);
        }

        [HttpGet("app/delete")]
        public async Task<IActionResult> DeleteApp(int id)
        {
            var appResult = await _appService.DeleteAppAsync(id);
            return LocalRedirect("~/app/all");
        }

        [HttpGet("app/{id}/secret")]
        public async Task<IActionResult> ChangeSecretApp(int id)
        {
            var appResult = await _appService.ChangeAppSecretAsync(id);
            if (appResult.IsSuccess)
                return View("AppDetails", appResult.Data);
            return LocalRedirect("~/app/all");
        }
    }
}
