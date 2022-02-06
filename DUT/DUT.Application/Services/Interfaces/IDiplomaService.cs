using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Diploma;
namespace DUT.Application.Services.Interfaces
{
    public interface IDiplomaService
    {
        Task<Result<bool>> CreateTemplatesAutomaticallyAsync();
        Task<Result<DiplomaViewModel>> CreateDiplomaBasicOnTemplateAsync(DiplomaCreateModel model, string templateId);
        Task<Result<DiplomaViewModel>> CreateDiplomaTemplateAsync(DiplomaTemplateCreateModel model);
        Task<Result<DiplomaViewModel>> UpdateDiplomaTemplateAsync(DiplomaTemplateEditModel model);
        Task<Result<List<DiplomaViewModel>>> GetDiplomaTemplatesAsync();
        Task<Result<List<DiplomaViewModel>>> GetUserDiplomasAsync(int userId);
        Task<Result<bool>> RemoveDiplomaAsync(string diplomaId);
    }
}