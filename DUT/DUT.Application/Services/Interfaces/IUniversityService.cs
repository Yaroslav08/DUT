using DUT.Application.ViewModels;
using DUT.Application.ViewModels.University;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface IUniversityService : IBaseService<University>
    {
        Task<Result<UniversityViewModel>> GetUniversityAsync();
        Task<Result<UniversityViewModel>> CreateUniversityAsync(UniversityCreateModel model);
        Task<Result<UniversityViewModel>> UpdateUniversityAsync(UniversityEditModel model);
    }
}
