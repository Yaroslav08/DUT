using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Apps;

namespace DUT.Application.Services.Interfaces
{
    public interface IAppService
    {
        Task<Result<List<AppViewModel>>> GetAllAppsAsync();
        Task<Result<AppViewModel>> GetAppByIdAsync(int id);
        Task<Result<AppViewModel>> CreateAppAsync(AppCreateModel app);
        Task<Result<AppViewModel>> UpdateAppAsync(AppEditModel app);
        Task<Result<AppViewModel>> ChangeAppSecretAsync(int appId);
        Task<Result<AppViewModel>> DeleteAppAsync(int id);
    }
}