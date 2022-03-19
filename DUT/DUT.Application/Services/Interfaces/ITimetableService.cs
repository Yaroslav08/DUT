using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Timetable;
using DUT.Domain.Models;
namespace DUT.Application.Services.Interfaces
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