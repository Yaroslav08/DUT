using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Specialty;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
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