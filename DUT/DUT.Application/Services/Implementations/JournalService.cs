using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Lesson;
using DUT.Constants.Extensions;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Force.DeepCloner;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class JournalService : IJournalService
    {
        private readonly char[] avalible = new char[] { 'н', 'н', 'n' };
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly DUTDbContext _db;
        public JournalService(IMapper mapper, DUTDbContext db, IIdentityService identityService)
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

            if (!TryMapMarksInJournal(lesson.Journal, journal, out var error))
            {
                return Result<LessonViewModel>.Error(error);
            }

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



        private bool TryMapMarksInJournal(Journal currentJournal, Journal newJournal, out string error)
        {
            var res = newJournal.Students.Select(s => s.Id).Except(currentJournal.Students.Select(s => s.Id));

            if (res != null && res.Count() > 0)
            {
                if (res.Count() == 1)
                    error = $"Студент {newJournal.Students.FirstOrDefault(s => s.Id == res.First()).Name} не існує";
                else
                    error = $"Студентів ({string.Join(",", newJournal.Students.Where(s => res.Contains(s.Id)).Select(s => s.Name))}) не існує";
                return false;
            }

            foreach (var student in newJournal.Students)
            {
                if (!ValidateMark(student.Mark))
                {
                    error = $"Оцінка {student.Mark} не є доступною";
                    return false;
                }
                currentJournal.Students.FirstOrDefault(s => s.Id == student.Id).Mark = student.Mark;
            }

            currentJournal.Statistics = GetJournalStatistics(currentJournal);

            error = null;
            return true;
        }

        private bool ValidateMark(string mark)
        {
            if (string.IsNullOrEmpty(mark))
                return true;
            if (char.IsLetter(mark[0]))
            {
                mark = mark.ToLower();
                return avalible.Contains(mark[0]);
            }
            if (char.IsDigit(mark[0]))
            {
                var digitMark = Convert.ToInt32(mark);
                return digitMark > 0;
            }
            return false;
        }

        private JournalStatistics GetJournalStatistics(Journal journal)
        {
            var countOfStudents = journal.Students.Count;
            var countOfExist = countOfStudents - journal.Students.Count(s => s.Mark != null && avalible.Contains(s.Mark[0]));

            var countWithMarks = journal.Students.Count(s => int.TryParse(s.Mark, out var markNumber) && markNumber > 0);
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
