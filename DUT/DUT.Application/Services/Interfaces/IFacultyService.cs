using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Faculty;

namespace DUT.Application.Services.Interfaces
{
    public interface IFacultyService
    {
        Task<Result<FacultyViewModel>> CreateFacultyAsync(FacultyCreateModel model);
        Task<Result<FacultyViewModel>> EditFacultyAsync(FacultyEditModel model);
        Task<Result<FacultyViewModel>> GetFacultyByIdAsync(int id);
        Task<Result<List<FacultyViewModel>>> GetAllFacultiesAsync();
    }
}
