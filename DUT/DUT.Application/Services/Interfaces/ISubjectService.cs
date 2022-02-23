using DUT.Application.Options;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Subject;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
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