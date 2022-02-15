using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Setting;
using DUT.Domain.Models;
namespace DUT.Application.Services.Interfaces
{
    public interface ISettingService : IBaseService<Setting>
    {
        Task<Result<Setting>> GetRootSettingAsync();
        Task<Result<SettingViewModel>> CreateSettingAsync(SettingCreateModel model);
        Task<Result<SettingViewModel>> UpdateSettingAsync(SettingEditModel model);
    }
}