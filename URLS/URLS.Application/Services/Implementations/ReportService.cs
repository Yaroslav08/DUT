using AutoMapper;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Report;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;
using Force.DeepCloner;
using Microsoft.EntityFrameworkCore;
namespace URLS.Application.Services.Implementations
{
    public class ReportService : BaseService<Report>, IReportService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ISubjectService _subjectService;
        public ReportService(URLSDbContext db, IMapper mapper, IIdentityService identityService, ISubjectService subjectService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
            _subjectService = subjectService;
        }

        public async Task<Result<ReportViewModel>> CreateReportAsync(int subjectId)
        {
            if (!await _subjectService.IsExistAsync(s => s.Id == subjectId))
                return Result<ReportViewModel>.NotFound(typeof(Subject).NotFoundMessage(subjectId));

            var allLessons = await _db.Lessons
                .AsNoTracking()
                .Where(s => s.SubjectId == subjectId)
                .OrderBy(s => s.Id)
                .ToListAsync();

            if (allLessons == null || allLessons.Count == 0)
                return Result<ReportViewModel>.Error("This subject don`t have any lessons");

            var now = DateTime.Now;

            var newReport = new Report
            {
                Title = $"Відомість від {now.ToString("dd.MM.yyyy (HH:mm)")}",
                IsDraft = true,
                SubjectId = subjectId,
                Type = ReportType.Intermediate
            };

            newReport.CalculatedMarks = await GetCalculatedMarksAsync(allLessons, _subjectService.Exists.First().GroupId.Value);

            newReport.PrepareToCreate(_identityService);

            await _db.Reports.AddAsync(newReport);
            await _db.SaveChangesAsync();

            var reportToView = _mapper.Map<ReportViewModel>(newReport);

            return Result<ReportViewModel>.SuccessWithData(reportToView);
        }

        public async Task<Result<ReportViewModel>> GetReportIdAsync(int subjectId, int id)
        {
            var report = await _db.Reports.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (report == null)
                return Result<ReportViewModel>.NotFound(typeof(Report).NotFoundMessage(id));

            if (report.SubjectId != subjectId)
                return Result<ReportViewModel>.Error("This report not from this subject");

            var reportToView = _mapper.Map<ReportViewModel>(report);
            return Result<ReportViewModel>.SuccessWithData(reportToView);
        }

        public async Task<Result<List<ReportViewModel>>> GetReportsBySubjectIdAsync(int subjectId)
        {
            var reports = await _db.Reports
                .AsNoTracking()
                .Where(s => s.SubjectId == subjectId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            var reportsToView = _mapper.Map<List<ReportViewModel>>(reports);
            return Result<List<ReportViewModel>>.SuccessWithData(reportsToView);
        }

        public async Task<Result<bool>> RemoveReportAsync(int subjectId, int id)
        {
            var reportToRemove = await _db.Reports.FirstOrDefaultAsync(s => s.Id == id);
            if (reportToRemove == null)
                return Result<bool>.NotFound(typeof(Report).NotFoundMessage(id));

            if (reportToRemove.SubjectId != subjectId)
                return Result<bool>.Error("This report not in this subject");

            _db.Reports.Remove(reportToRemove);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<ReportViewModel>> UpdateReportAsync(ReportEditModel model)
        {
            var currentReport = await _db.Reports.AsNoTracking().FirstOrDefaultAsync(s => s.Id == model.Id);
            if (currentReport == null)
                return Result<ReportViewModel>.NotFound(typeof(Report).NotFoundMessage(model.Id));

            if (currentReport.SubjectId != model.SubjectId)
                return Result<ReportViewModel>.Error("This report not in this subject");

            if (!currentReport.IsDraft && currentReport.Type == ReportType.Finall)
                return Result<ReportViewModel>.Error("Can`t edit final report");

            if (model.Type == ReportType.Finall)
                if (await IsExistAsync(s => s.SubjectId == model.SubjectId && s.Type == ReportType.Finall))
                    return Result<ReportViewModel>.Error("A subject must have only one final report");

            currentReport.Title = model.Title;
            currentReport.IsDraft = model.IsDraft;
            currentReport.Type = model.Type;

            if (!TrySynchronizeMarkMatrix(currentReport, model, out var error))
            {
                return Result<ReportViewModel>.Error(error);
            }

            currentReport.PrepareToUpdate(_identityService);
            _db.Reports.Update(currentReport);
            await _db.SaveChangesAsync();

            var updatedReport = _mapper.Map<ReportViewModel>(currentReport);

            return Result<ReportViewModel>.SuccessWithData(updatedReport);
        }

        private async Task<List<Student>> GetCalculatedMarksAsync(List<Lesson> lessons, int groupId)
        {
            var students = await _db.UserGroups
                .AsNoTracking()
                .Where(s => s.GroupId == groupId && s.Status == UserGroupStatus.Member && !s.IsAdmin)
                .Include(s => s.User)
                .Select(s => s.User)
                .ToListAsync();

            var listOfStudents = students.OrderBy(s => s.LastName).Select(s => new Student
            {
                Id = s.Id,
                Name = $"{s.LastName} {s.FirstName}"
            }).ToList();

            foreach (var student in listOfStudents)
            {
                student.Mark = GetMarksForStudent(student.Id, lessons);
            }

            return listOfStudents;
        }

        private string GetMarksForStudent(int studentId, List<Lesson> lessons)
        {
            int finallMark = 0;

            foreach (var lesson in lessons)
            {
                var mark = lesson.Journal?.Students?.FirstOrDefault(s => s.Id == studentId).Mark;
                if (int.TryParse(mark, out var Mark) && Mark > 0)
                    finallMark += Mark;
            }

            return finallMark.ToString();
        }

        private bool TrySynchronizeMarkMatrix(Report currentReport, ReportEditModel reportEditModel, out string error)
        {
            if (reportEditModel.Marks == null || reportEditModel.Marks.Count == 0)
            {
                error = "List of marks can't be empty";
                return false;
            }

            //var diff = reportEditModel.Marks.Select(s => s.Id).Except(currentReport.Marks?.Select(s => s.Id));
            //if (diff != null || diff.Count() > 0)
            //{
            //    error = $"Items with ID ({string.Join(",", diff)}) not valid";
            //    return false;
            //}

            if (currentReport.Type == ReportType.Finall)
            {
                currentReport.Marks = new List<Student>();
                foreach (var studentMark in currentReport.CalculatedMarks)
                {
                    var finallMark = studentMark.DeepClone();
                    var mark = reportEditModel.Marks?.FirstOrDefault(s => s.Id == studentMark.Id);
                    finallMark.Mark = mark?.Mark;
                    currentReport.Marks.Add(finallMark);
                }
            }

            error = null;
            return true;
        }
    }
}