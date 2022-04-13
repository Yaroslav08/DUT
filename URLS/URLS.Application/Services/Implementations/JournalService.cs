﻿using AutoMapper;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Lesson;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;
using Force.DeepCloner;
using Microsoft.EntityFrameworkCore;

namespace URLS.Application.Services.Implementations
{
    public class JournalService : IJournalService
    {
        private readonly char[] avalible = new char[] { 'н', 'н', 'n' };
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly URLSDbContext _db;
        public JournalService(IMapper mapper, URLSDbContext db, IIdentityService identityService)
        {
            _mapper = mapper;
            _db = db;
            _identityService = identityService;
        }

        public async Task<Result<LessonViewModel>> CreateJournalAsync(int subjectId, long lessonId)
        {
            var lesson = await _db.Lessons.AsNoTracking().FirstOrDefaultAsync(s => s.Id == lessonId);
            if (lesson == null)
                return Result<LessonViewModel>.NotFound("Lesson not found");

            if (lesson.SubjectId != subjectId)
                return Result<LessonViewModel>.Error("Lesson not in this subject");

            var subject = await _db.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == subjectId);
            if (subject == null)
                return Result<LessonViewModel>.NotFound("Subject not found");

            await FillJournalAsync(lesson, subject.GroupId.Value);

            lesson.PrepareToUpdate(_identityService);

            _db.Lessons.Update(lesson);
            await _db.SaveChangesAsync();

            var lessonToView = _mapper.Map<LessonViewModel>(lesson);

            return Result<LessonViewModel>.SuccessWithData(lessonToView);
        }

        public async Task<Result<LessonViewModel>> SynchronizeJournalAsync(int subjectId, long lessonId)
        {
            var lesson = await _db.Lessons.AsNoTracking().FirstOrDefaultAsync(s => s.Id == lessonId);
            if (lesson == null)
                return Result<LessonViewModel>.NotFound("Lesson not found");

            if (lesson.SubjectId != subjectId)
                return Result<LessonViewModel>.Error("Lesson not in this subject");

            var subject = await _db.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == subjectId);
            if (subject == null)
                return Result<LessonViewModel>.NotFound("Subject not found");

            await FillJournalAsync(lesson, subject.GroupId.Value);

            lesson.PrepareToUpdate(_identityService);
            _db.Lessons.Update(lesson);
            await _db.SaveChangesAsync();

            var synchronizedJournal = _mapper.Map<LessonViewModel>(lesson);

            return Result<LessonViewModel>.SuccessWithData(synchronizedJournal);
        }

        public async Task<Result<LessonViewModel>> UpdateJournalAsync(int subjectId, long lessonId, Journal journal)
        {
            var lesson = await _db.Lessons.AsNoTracking().FirstOrDefaultAsync(s => s.Id == lessonId);
            if (lesson == null)
                return Result<LessonViewModel>.NotFound(typeof(Lesson).NotFoundMessage(lessonId));

            if (lesson.SubjectId != subjectId)
                return Result<LessonViewModel>.Error("Lesson not in this subject");

            var subject = await _db.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == subjectId);
            if (subject == null)
                return Result<LessonViewModel>.NotFound("Subject not found");

            if (lesson.Journal == null)
                await FillJournalAsync(lesson, subject.GroupId.Value);

            var res = await MapMarksInJournalAsync(subject, lesson, journal);
            if (!res.Success)
                return Result<LessonViewModel>.Error(res.Error);

            lesson.PrepareToUpdate(_identityService);

            _db.Lessons.Update(lesson);
            await _db.SaveChangesAsync();

            var updatedLesson = _mapper.Map<LessonViewModel>(lesson);
            return Result<LessonViewModel>.SuccessWithData(updatedLesson);
        }

        public async Task<Result<LessonViewModel>> RemoveJournalAsync(int subjectId, long lessonId)
        {
            var lesson = await _db.Lessons.AsNoTracking().FirstOrDefaultAsync(s => s.Id == lessonId);
            if (lesson == null)
                return Result<LessonViewModel>.NotFound(typeof(Lesson).NotFoundMessage(lessonId));

            if (lesson.SubjectId != subjectId)
                return Result<LessonViewModel>.Error("Lesson not in this subject");

            lesson.Journal = null;
            lesson.PrepareToUpdate(_identityService);

            _db.Lessons.Update(lesson);
            await _db.SaveChangesAsync();

            var updatedLesson = _mapper.Map<LessonViewModel>(lesson);

            return Result<LessonViewModel>.SuccessWithData(updatedLesson);
        }

        public async Task<Result<Journal>> GetJournalAsync(int subjectId, long lessonId)
        {
            var lesson = await _db.Lessons.AsNoTracking().FirstOrDefaultAsync(s => s.Id == lessonId);
            if (lesson == null)
                return Result<Journal>.NotFound(typeof(Lesson).NotFoundMessage(lessonId));

            if (lesson.SubjectId != subjectId)
                return Result<Journal>.Error("Lesson not in this subject");

            return Result<Journal>.SuccessWithData(lesson.Journal);
        }



        private async Task<(bool Success, string Error)> MapMarksInJournalAsync(Subject subject, Lesson currentLesson, Journal newJournal)
        {
            string error = null;

            var currentJournal = currentLesson.Journal;

            var res = newJournal.Students.Select(s => s.Id).Except(currentJournal.Students.Select(s => s.Id));

            if (res != null && res.Count() > 0)
            {
                if (res.Count() == 1)
                    error = $"Студент {newJournal.Students.FirstOrDefault(s => s.Id == res.First()).Name} не існує";
                else
                    error = $"Студентів ({string.Join(",", newJournal.Students.Where(s => res.Contains(s.Id)).Select(s => s.Name))}) не існує";
                return (false, error);
            }

            var previewLessons = await _db.Lessons
                .AsNoTracking()
                .Where(s => s.SubjectId == subject.Id && s.Date < currentLesson.Date)
                .OrderBy(s => s.CreatedAt)
                .ToListAsync();

            foreach (var student in newJournal.Students)
            {
                if (!TryValidateMark(student.Mark, subject.Config, out var validError))
                {
                    error = validError;
                    return (false, error);
                }


                var currentSumOfMarks = GetStudentMarksUpToNow(previewLessons, student.Id);

                if (subject.Config.WithExam)
                {
                    if (currentLesson.LessonType != LessonType.Exam)
                    {
                        if (currentSumOfMarks == subject.Config.MaxMarkUpToExam)
                        {
                            error = $"{currentJournal.Students.First(s => s.Id == student.Id).Name} is already have max mark";
                            return (false, error);
                        }
                        if (double.TryParse(student.Mark, out var numberMark))
                        {
                            var diff = subject.Config.MaxMarkUpToExam - currentSumOfMarks;

                            if (diff < numberMark)
                            {
                                student.Mark = diff.ToString();
                            }
                        }
                    }
                    else
                    {
                        if (double.TryParse(student.Mark, out var numberMark))
                        {
                            if (numberMark > subject.Config.MaxMarkInExam)
                            {
                                error = $"Max mark in exam = {subject.Config.MaxMarkInExam}";
                                return (false, error);
                            }
                        }
                    }
                }
                else
                {
                    if (double.TryParse(student.Mark, out var numberMark))
                    {
                        var diff = subject.Config.MaxMark - currentSumOfMarks;
                        if (diff < numberMark)
                        {
                            student.Mark = (numberMark - diff).ToString();
                        }
                    }
                }
                currentJournal.Students.FirstOrDefault(s => s.Id == student.Id).Mark = student.Mark;
            }

            currentJournal.Statistics = GetJournalStatistics(currentJournal);

            error = null;
            return (true, error);
        }

        private double GetStudentMarksUpToNow(List<Lesson> lessons, int studentId)
        {
            var studentMarks = lessons
                .Select(s => s.Journal)
                .Select(s => s.Students.FirstOrDefault(s => s.Id == studentId))
                .Where(s => double.TryParse(s.Mark, out var numberMark))
                .Sum(s => Convert.ToDouble(s.Mark));

            return studentMarks;
        }

        private bool TryValidateMark(string mark, SubjectConfig config, out string error)
        {
            if (string.IsNullOrEmpty(mark))
            {
                error = null;
                return true;
            }
            if (char.IsLetter(mark[0]))
            {
                mark = mark.Substring(0, 1).ToLower();
                var res = avalible.Contains(mark[0]);
                error = res ? null : "Isn't valid mark";
                return res;
            }
            if (double.TryParse(mark, out var digitMark))
            {
                if (digitMark < config.MinMarkPerLesson)
                {
                    error = $"Mark can't less then {config.MinMarkPerLesson}";
                    return false;
                }
                if (digitMark > config.MaxMarkPerLesson)
                {
                    error = $"Mark can't more then {config.MaxMarkPerLesson}";
                    return false;
                }
                error = null;
                return true;
            }
            error = "Some error";
            return false;
        }

        private JournalStatistics GetJournalStatistics(Journal journal)
        {
            var countOfStudents = journal.Students.Count;
            var countOfExist = countOfStudents - journal.Students.Count(s => s.Mark != null && avalible.Contains(s.Mark[0]));

            var countWithMarks = journal.Students.Count(s => double.TryParse(s.Mark, out var markNumber) && markNumber > 0);
            var countWithoutMarks = countOfStudents - countWithMarks;

            return new JournalStatistics
            {
                CountOfStudents = countOfStudents,
                CountOfExist = countOfExist,
                CountWithMarks = countWithMarks,
                CountWithoutMarks = countWithoutMarks
            };
        }

        private async Task FillJournalAsync(Lesson lesson, int groupId)
        {
            var students = await _db.UserGroups
                .AsNoTracking()
                .Where(s => s.GroupId == groupId && s.Status == UserGroupStatus.Member && !s.IsAdmin)
                .Include(s => s.User)
                .Select(s => s.User)
                .ToListAsync();
            students = students.OrderBy(s => s.LastName).ToList();

            var isNew = lesson.Journal == null;

            if (isNew)
            {
                lesson.Journal = new Journal
                {
                    Students = students.Select(s => new Student
                    {
                        Id = s.Id,
                        Name = $"{s.LastName} {s.FirstName}",
                        Mark = null
                    }).ToList(),
                    Statistics = null
                };
            }
            else
            {
                var oldJournal = lesson.Journal.DeepClone();

                var newJournal = new Journal
                {
                    Students = students.Select(student => new Student
                    {
                        Id = student.Id,
                        Name = $"{student.LastName} {student.FirstName}",
                        Mark = oldJournal.Students.FirstOrDefault(s => s.Id == student.Id)?.Mark
                    }).ToList()
                };

                lesson.Journal = newJournal;
                lesson.Journal.Statistics = GetJournalStatistics(newJournal);
            }
        }
    }
}
