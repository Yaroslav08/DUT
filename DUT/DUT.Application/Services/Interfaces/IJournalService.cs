using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Lesson;
using DUT.Domain.Models;
namespace DUT.Application.Services.Interfaces
{
    public interface IJournalService
    {
        Task<Result<LessonViewModel>> CreateJournalAsync(int subjectId, long lessonId);
        Task<Result<LessonViewModel>> UpdateJournalAsync(int subjectId, long lessonId, Journal journal);
        Task<Result<LessonViewModel>> RemoveJournalAsync(int subjectId, long lessonId);
        Task<Result<LessonViewModel>> SynchronizeJournalAsync(int subjectId, long lessonId);
    }
}