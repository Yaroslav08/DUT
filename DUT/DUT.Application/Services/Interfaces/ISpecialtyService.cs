using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Specialty;

namespace DUT.Application.Services.Interfaces
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