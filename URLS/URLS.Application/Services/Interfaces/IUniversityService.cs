using URLS.Application.ViewModels;
using URLS.Application.ViewModels.University;
using URLS.Domain.Models;
namespace URLS.Application.Services.Interfaces
{
    public interface IUniversityService : IBaseService<University>
    {
        Task<Result<UniversityViewModel>> GetUniversityAsync();
        Task<Result<UniversityViewModel>> CreateUniversityAsync(UniversityCreateModel model);
        Task<Result<UniversityViewModel>> UpdateUniversityAsync(UniversityEditModel model);
    }
}