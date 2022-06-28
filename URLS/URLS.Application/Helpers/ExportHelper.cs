using ClosedXML.Excel;
using URLS.Application.ViewModels.Export;
using URLS.Domain.Models;

namespace URLS.Application.Helpers
{
    public static class ExportHelper
    {
        public static ExportViewModel GetGroupExportFile(Group group, List<UserGroup> userGroups)
        {
            using var workbook = new XLWorkbook();
            workbook.Style.Font.FontSize = 14;

            var workShitGroup = workbook.AddWorksheet("Група");

            workShitGroup.Cell(1, 1).Value = "Назва:";
            workShitGroup.Cell(1, 2).Value = group.Name;
            workShitGroup.Cell(2, 1).Value = "Курс:";
            workShitGroup.Cell(2, 2).Value = $"{group.Course} курс";
            workShitGroup.Cell(3, 1).Value = "Початок навчання:";
            workShitGroup.Cell(3, 2).Value = group.StartStudy;
            workShitGroup.Cell(4, 1).Value = "Кінець навчання:";
            workShitGroup.Cell(4, 2).Value = group.EndStudy.ToString("dd.MM.yyyy р.");

            workShitGroup.Cell(6, 1).Value = "ПІБ куратора:";
            var admin = userGroups.FirstOrDefault(s => s.IsAdmin);
            workShitGroup.Cell(6, 2).Value = $"{admin.User.LastName} {admin.User.FirstName}";
            workShitGroup.Cell(7, 1).Value = "Куратор від:";
            workShitGroup.Cell(7, 2).Value = admin.CreatedAt.ToString("HH:mm dd.MM.yyyy");



            var workShitMembers = workbook.AddWorksheet("Студенти");

            workShitMembers.Cell(1, 1).Value = "Ім'я";
            workShitMembers.Cell(1, 2).Value = "Прізвище";
            workShitMembers.Cell(1, 3).Value = "Підпис";
            workShitMembers.Cell(1, 4).Value = "Роль";
            workShitMembers.Cell(1, 5).Value = "Статус";

            var row = 2;
            foreach (var member in userGroups.OrderBy(s => s.User.LastName).Where(s => !s.IsAdmin))
            {
                workShitMembers.Cell(row, 1).Value = member.User.FirstName;
                workShitMembers.Cell(row, 2).Value = member.User.LastName;
                workShitMembers.Cell(row, 3).Value = member.Title;
                workShitMembers.Cell(row, 4).Value = member.UserGroupRole.Name;
                workShitMembers.Cell(row, 5).Value = member.Status.ToString();
                row++;
            }


            var fileName = $"Група - {group.Name.ToUpper()} ({DateTime.Now.ToString("HH:mm dd-MM-yyyy")}).xlsx";

            var exportModel = new ExportViewModel
            {
                FileName = fileName,
                Stream = new MemoryStream()
            };

            workbook.SaveAs(exportModel.Stream);
            exportModel.Stream.Position = 0;

            return exportModel;
        }
    }
}
