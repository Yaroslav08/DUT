using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Helpers;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Export;
using URLS.Constants.Extensions;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class ExportService : IExportService
    {
        private readonly URLSDbContext _db;
        private readonly ICommonService _commonService;
        public ExportService(URLSDbContext db, ICommonService commonService)
        {
            _db = db;
            _commonService = commonService;
        }

        public async Task<Result<ExportViewModel>> GetGroupExportAsync(int groupId)
        {
            var isExistGroup = await _commonService.IsExistWithResultsAsync<Domain.Models.Group>(s => s.Id == groupId);

            if (!isExistGroup.IsExist)
                return Result<ExportViewModel>.NotFound(typeof(Domain.Models.Group).NotFoundMessage(groupId));

            var group = isExistGroup.Results.First();

            var groupMembers = await _db.UserGroups
                .Where(s => s.GroupId == groupId)
                .Include(s => s.User)
                .Include(s => s.UserGroupRole)
                .ToListAsync();

            var exportModel = ExportHelper.GetGroupExportFile(group, groupMembers);

            return Result<ExportViewModel>.SuccessWithData(exportModel);
        }
    }
}