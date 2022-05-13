using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Specialty;
namespace URLS.Application.Services.Interfaces
{
    public interface ISpecialtyService
    {
        Task<Result<SpecialtyViewModel>> CreateSpecialtyAsync(SpecialtyCreateModel model);
        Task<Result<SpecialtyViewModel>> UpdateSpecialtyAsync(SpecialtyEditModel model);
        Task<Result<SpecialtyViewModel>> GetSpecialtyByIdAsync(int id);
        Task<Result<List<SpecialtyViewModel>>> GetAllSpecialtiesAsync();
        Task<Result<List<SpecialtyViewModel>>> GetSpecialtiesByFacultyIdAsync(int facultyId);
    }
}