using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Specialty;
using URLS.Domain.Models;
namespace URLS.Application.Services.Interfaces
{
    public interface ISpecialtyService : IBaseService<Specialty>
    {
        Task<Result<SpecialtyViewModel>> CreateSpecialtyAsync(SpecialtyCreateModel model);
        Task<Result<SpecialtyViewModel>> UpdateSpecialtyAsync(SpecialtyEditModel model);
        Task<Result<SpecialtyViewModel>> GetSpecialtyByIdAsync(int id);
        Task<Result<List<SpecialtyViewModel>>> GetAllSpecialtiesAsync();
        Task<Result<List<SpecialtyViewModel>>> GetSpecialtiesByFacultyIdAsync(int facultyId);
    }
}