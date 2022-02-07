using DUT.Application.Options;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Subject;
namespace DUT.Application.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<Result<List<SubjectViewModel>>> GetAllSubjectsAsync(SearchSubjectOptions options);
        Task<Result<List<SubjectViewModel>>> SearchSubjectsAsync(SearchSubjectOptions options);
        Task<Result<List<SubjectViewModel>>> GetAllTemplatesAsync();
        Task<Result<SubjectViewModel>> GetSubjectByIdAsync(int subjectId);
        Task<Result<SubjectViewModel>> CreateSubjectAsync(SubjectCreateModel model);
    }
}