using URLS.Application.Options;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Subject;
using URLS.Domain.Models;

namespace URLS.Application.Services.Interfaces
{
    public interface ISubjectService : IBaseService<Subject>
    {
        Task<Result<List<SubjectViewModel>>> SearchSubjectsAsync(SearchSubjectOptions options);
        Task<Result<SubjectViewModel>> GetSubjectByIdAsync(int subjectId);
        Task<Result<SubjectViewModel>> GetGroupSubjectAsync(int groupId, int subjectId);
        Task<Result<SubjectViewModel>> CreateSubjectAsync(SubjectCreateModel model);
        Task<Result<SubjectViewModel>> UpdateSubjectAsync(SubjectEditModel model);
    }
}