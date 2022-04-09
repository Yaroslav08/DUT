using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Faculty;
using URLS.Domain.Models;
namespace URLS.Application.Services.Interfaces
{
    public interface IFacultyService : IBaseService<Faculty>
    {
        Task<Result<FacultyViewModel>> CreateFacultyAsync(FacultyCreateModel model);
        Task<Result<FacultyViewModel>> UpdateFacultyAsync(FacultyEditModel model);
        Task<Result<FacultyViewModel>> GetFacultyByIdAsync(int id);
        Task<Result<List<FacultyViewModel>>> GetAllFacultiesAsync();
        Task<Result<List<FacultyViewModel>>> GetFacultiesByUniversityIdAsync(int id);
    }
}