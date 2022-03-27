using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Report;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface IReportService : IBaseService<Report>
    {
        Task<Result<ReportViewModel>> CreateReportAsync(int subjectId);
        Task<Result<List<ReportViewModel>>> GetReportsBySubjectIdAsync(int subjectId);
        Task<Result<ReportViewModel>> UpdateReportAsync(ReportEditModel model);
        Task<Result<ReportViewModel>> GetReportIdAsync(int subjectId, int id);
        Task<Result<bool>> RemoveReportAsync(int subjectId, int id);
    }
}