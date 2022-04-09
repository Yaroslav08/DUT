using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Timetable;
using URLS.Domain.Models;
namespace URLS.Application.Services.Interfaces
{
    public interface ITimetableService : IBaseService<Timetable>
    {
        Task<Result<List<TimetableViewModel>>> GetTimetableBetweenDatesAsync(int groupId, DateTime startDate, DateTime endDate);
        Task<Result<TimetableViewModel>> CreateTimetableAsync(TimetableCreateModel model);
        Task<Result<TimetableViewModel>> UpdateTimetableAsync(TimetableCreateModel model);
        Task<Result<bool>> RemoveTimetableAsync(long[] ids);
        Task<Result<bool>> RemoveTimetableAsync(int? groupId, int? subjectId, DateTime from, DateTime to);
    }
}