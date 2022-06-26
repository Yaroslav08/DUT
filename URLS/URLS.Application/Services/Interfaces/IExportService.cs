using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Export;

namespace URLS.Application.Services.Interfaces
{
    public interface IExportService
    {
        Task<Result<ExportViewModel>> GetGroupMemberExportAsync(int groupId);
    }
}
