using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Diploma;

namespace DUT.Application.Services.Interfaces
{
    public interface IDiplomaService
    {
        Task<Result<DiplomaViewModel>> OrderDiplomaAsync(int userId);
        Task<Result<DiplomaViewModel>> ReorderDiplomaAsync(int userId, string diplomaId);
        Task<Result<List<DiplomaViewModel>>> GetUserDiplomasAsync(int userId);
    }
}