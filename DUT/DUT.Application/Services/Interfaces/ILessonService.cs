using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Lesson;
using DUT.Domain.Models;
namespace DUT.Application.Services.Interfaces
{
    public interface ILessonService : IBaseService<Lesson>
    {
        Task<Result<List<LessonViewModel>>> GetLessonsBySubjectIdAsync(int subjectId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<Result<LessonViewModel>> GetLessonByIdAsync(long id);
        Task<Result<LessonViewModel>> CreateLessonAsync(LessonCreateModel lesson);
        Task<Result<LessonViewModel>> UpdateLessonAsync(LessonEditModel lesson);
        Task<Result<bool>> RemoveLessonAsync(long id);
    }
}