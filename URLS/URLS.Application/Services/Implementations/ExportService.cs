﻿using Microsoft.EntityFrameworkCore;
using URLS.Application.Helpers;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Export;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
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

        public async Task<Result<ExportViewModel>> ExportMarksBySubjectIdAsync(int subjectId)
        {
            if (!await _commonService.IsExistAsync<Subject>(s => s.Id == subjectId))
                return Result<ExportViewModel>.NotFound(typeof(Subject).NotFoundMessage(subjectId));

            var subject = await _db.Subjects
                .AsNoTracking()
                .Include(s => s.Lessons)
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Id == subjectId);

            var exportModel = ExportHelper.ExportMarksFile(subject, subject.From, subject.To);

            return Result<ExportViewModel>.SuccessWithData(exportModel);
        }

        public async Task<Result<ExportViewModel>> ExportGroupAsync(int groupId)
        {
            var isExistGroup = await _commonService.IsExistWithResultsAsync<Group>(s => s.Id == groupId);

            if (!isExistGroup.IsExist)
                return Result<ExportViewModel>.NotFound(typeof(Group).NotFoundMessage(groupId));

            var group = isExistGroup.Results.First();

            var groupMembers = await _db.UserGroups
                .Where(s => s.GroupId == groupId)
                .Include(s => s.User)
                .Include(s => s.UserGroupRole)
                .ToListAsync();

            var exportModel = ExportHelper.ExportGroupFile(group, groupMembers);

            return Result<ExportViewModel>.SuccessWithData(exportModel);
        }

        public async Task<Result<ExportViewModel>> ExportLessonMarkAsync(int subjectId, long lessonId)
        {
            var isExistLesson = await _commonService.IsExistWithResultsAsync<Lesson>(s => s.Id == lessonId);

            if (!isExistLesson.IsExist)
                return Result<ExportViewModel>.NotFound(typeof(Lesson).NotFoundMessage(lessonId));

            var lesson = isExistLesson.Results.First();

            if (lesson.SubjectId != subjectId)
                return Result<ExportViewModel>.Error("Lesson not releted with subject");

            lesson.Subject = await _db.Subjects.AsNoTracking().Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == subjectId);

            var exportModel = ExportHelper.ExportLessonMarkFile(lesson);

            return Result<ExportViewModel>.SuccessWithData(exportModel);
        }

        public async Task<Result<ExportViewModel>> ExportMarksBySubjectIdAsync(int subjectId, DateTime from, DateTime to)
        {
            if (!await _commonService.IsExistAsync<Subject>(s => s.Id == subjectId))
                return Result<ExportViewModel>.NotFound(typeof(Subject).NotFoundMessage(subjectId));

            var subject = await _db.Subjects
                .AsNoTracking()
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Id == subjectId);

            subject.Lessons = await _db.Lessons.Where(s => s.SubjectId == subjectId && (s.Date > from && s.Date < to)).ToListAsync();

            var exportModel = ExportHelper.ExportMarksFile(subject, from, to);

            return Result<ExportViewModel>.SuccessWithData(exportModel);
        }
    }
}