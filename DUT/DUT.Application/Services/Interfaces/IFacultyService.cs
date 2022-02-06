using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Faculty;
using DUT.Application.ViewModels.Specialty;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface IFacultyService : IBaseService<Faculty>
    {
        Task<Result<FacultyViewModel>> CreateFacultyAsync(FacultyCreateModel model);
        Task<Result<FacultyViewModel>> UpdateFacultyAsync(FacultyEditModel model);
        Task<Result<FacultyViewModel>> GetFacultyByIdAsync(int id);
        Task<Result<List<FacultyViewModel>>> GetAllFacultiesAsync();
        Task<Result<List<SpecialtyViewModel>>> GetSpecialtiesByFacultyIdAsync(int id);
    }
}