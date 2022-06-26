using ClosedXML.Excel;
using URLS.Application.ViewModels.Export;
using URLS.Domain.Models;

namespace URLS.Application.Helpers
{
    public static class ExportHelper
    {
        public static ExportViewModel GetGroupMemberExportFile(Group group, List<UserGroup> userGroups)
        {
            using var workbook = new XLWorkbook();









            var exportModel = new ExportViewModel
            {
                FileName = DateTime.Now.ToString("HH:mm dd-MM-yyyy") + ".xlsx",
                Stream = new MemoryStream()
            };

            workbook.SaveAs(exportModel.Stream);
            exportModel.Stream.Position = 0;

            return exportModel;
        }
    }
}
