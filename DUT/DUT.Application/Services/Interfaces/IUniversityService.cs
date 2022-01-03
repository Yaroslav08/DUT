using DUT.Application.ViewModels;
using DUT.Application.ViewModels.University;

namespace DUT.Application.Services.Interfaces
{
    public interface IUniversityService
    {
        Task<Result<UniversityViewModel>> GetUniversityAsync();
        Task<Result<UniversityViewModel>> CreateUniversityAsync(UniversityCreateModel model);
        Task<Result<UniversityViewModel>> UpdateUniversityAsync(UniversityEditModel model);
    }
}
