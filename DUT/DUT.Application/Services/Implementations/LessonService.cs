using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Lesson;
using DUT.Application.ViewModels.User;
using DUT.Constants.Extensions;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DUT.Application.Services.Implementations
{
    public class LessonService : BaseService<Lesson>, ILessonService
    {
        private readonly IIdentityService _identityService;
        private readonly ISubjectService _subjectService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly DUTDbContext _db;
        public LessonService(IIdentityService identityService, ISubjectService subjectService, IMapper mapper, DUTDbContext db, IUserService userService) : base(db)
        {
            _identityService = identityService;
            _subjectService = subjectService;
            _mapper = mapper;
            _db = db;
            _userService = userService;
        }

        public async Task<Result<LessonViewModel>> CreateLessonAsync(LessonCreateModel lesson)
        {
            if (!await _subjectService.IsExistAsync(s => s.Id == lesson.SubjectId))
                return Result<LessonViewModel>.NotFound(typeof(Subject).NotFoundMessage(lesson.SubjectId));

            if (lesson.SubstituteTeacherId.HasValue)
                if (!await _userService.IsExistAsync(s => s.Id == lesson.SubstituteTeacherId))
                    return Result<LessonViewModel>.NotFound(typeof(User).NotFoundMessage(lesson.SubstituteTeacherId));

            if (lesson.PreviewLessonId.HasValue)
                if (!await IsExistAsync(s => s.Id == lesson.PreviewLessonId))
                {
                    return Result<LessonViewModel>.NotFound(typeof(Lesson).NotFoundMessage(lesson.PreviewLessonId));
                }
                else
                {
                    var previewLesson = Exists.First();
                    if (previewLesson.SubjectId != lesson.SubjectId)
                        return Result<LessonViewModel>.Error("This lesson is not on this subject");
                }
            if (lesson.NextLessonId.HasValue)
                if (!await IsExistAsync(s => s.Id == lesson.NextLessonId))
                {
                    return Result<LessonViewModel>.NotFound(typeof(Lesson).NotFoundMessage(lesson.NextLessonId));
                }
                else
                {
                    var nextLesson = Exists.First();
                    if (nextLesson.SubjectId != lesson.SubjectId)
                        return Result<LessonViewModel>.Error("This lesson is not on this subject");
                }

            var newLesson = new Lesson
            {
                Theme = lesson.Theme,
                Description = lesson.Description,
                Date = lesson.Date,
                Homework = lesson.Homework,
                LessonType = lesson.LessonType,
                PreviewLessonId = lesson.PreviewLessonId,
                NextLessonId = lesson.NextLessonId,
                SubjectId = lesson.SubjectId,
                Journal = null,
                SubstituteTeacherId = lesson.SubstituteTeacherId,
            };
            newLesson.PrepareToCreate(_identityService);

            await _db.Lessons.AddAsync(newLesson);
            await _db.SaveChangesAsync();

            var lessonToView = _mapper.Map<LessonViewModel>(newLesson);

            return Result<LessonViewModel>.SuccessWithData(lessonToView);
        }

        public async Task<Result<LessonViewModel>> GetLessonByIdAsync(long id)
        {
            var lesson = await _db.Lessons.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (lesson == null)
                return Result<LessonViewModel>.NotFound(typeof(Lesson).NotFoundMessage(id));

            var lessonToView = _mapper.Map<LessonViewModel>(lesson);

            lessonToView.PreviewLesson = await GetSubLessonAsync(lesson.PreviewLessonId);
            lessonToView.NextLesson = await GetSubLessonAsync(lesson.NextLessonId);

            if (lesson.SubstituteTeacherId.HasValue)
            {
                lessonToView.IsSubstitute = true;
                lessonToView.SubstituteTeacher = await _db.Users.AsNoTracking().Select(s => new UserViewModel
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Image = s.Image,
                    JoinAt = s.JoinAt,
                    MiddleName = s.MiddleName,
                    FullName = $"{s.LastName} {s.FirstName} {s.MiddleName}",
                    UserName = s.UserName
                }).FirstOrDefaultAsync(s => s.Id == lesson.SubstituteTeacherId);
            }

            return Result<LessonViewModel>.SuccessWithData(lessonToView);
        }

        public async Task<Result<List<LessonViewModel>>> GetLessonsBySubjectIdAsync(int subjectId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            ValidateDate(ref fromDate, ref toDate);

            var lessons = await _db.Lessons
                .AsNoTracking()
                .Where(s => s.SubjectId == subjectId)
                .Where(s => s.Date >= fromDate && s.Date <= toDate)
                .OrderBy(s => s.Date)
                .ToListAsync();

            var lessonsToView = _mapper.Map<List<LessonViewModel>>(lessons);

            lessonsToView.ForEach(s =>
            {
                var lesson = lessons.First(x => x.Id == s.Id);
                if (lesson.SubstituteTeacherId.HasValue)
                    s.IsSubstitute = true;
            });

            return Result<List<LessonViewModel>>.SuccessWithData(lessonsToView);
        }

        public async Task<Result<bool>> RemoveLessonAsync(long id)
        {
            var lessonToRemove = await _db.Lessons.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (lessonToRemove == null)
                return Result<bool>.NotFound(typeof(Lesson).NotFoundMessage(id));

            _db.Lessons.Remove(lessonToRemove);
            await _db.SaveChangesAsync();

            return Result<bool>.SuccessWithData(true);
        }

        public async Task<Result<LessonViewModel>> UpdateLessonAsync(LessonEditModel lesson)
        {
            if (lesson.SubstituteTeacherId.HasValue)
                if (!await _userService.IsExistAsync(s => s.Id == lesson.SubstituteTeacherId))
                    return Result<LessonViewModel>.NotFound(typeof(User).NotFoundMessage(lesson.SubstituteTeacherId));

            var currentLesson = await _db.Lessons.AsNoTracking().FirstOrDefaultAsync(s => s.Id == lesson.Id);
            if (currentLesson == null)
                return Result<LessonViewModel>.NotFound(typeof(Lesson).NotFoundMessage(lesson.Id));

            if (lesson.PreviewLessonId.HasValue)
                if (!await IsExistAsync(s => s.Id == lesson.PreviewLessonId))
                {
                    return Result<LessonViewModel>.NotFound(typeof(Lesson).NotFoundMessage(lesson.PreviewLessonId));
                }
                else
                {
                    var previewLesson = Exists.First();
                    if (previewLesson.SubjectId != lesson.SubjectId)
                        return Result<LessonViewModel>.Error("This lesson is not on this subject");
                }
            if (lesson.NextLessonId.HasValue)
                if (!await IsExistAsync(s => s.Id == lesson.NextLessonId))
                {
                    return Result<LessonViewModel>.NotFound(typeof(Lesson).NotFoundMessage(lesson.NextLessonId));
                }
                else
                {
                    var nextLesson = Exists.First();
                    if (nextLesson.SubjectId != lesson.SubjectId)
                        return Result<LessonViewModel>.Error("This lesson is not on this subject");
                }

            currentLesson.Theme = lesson.Theme;
            currentLesson.Description = lesson.Description;
            currentLesson.Date = lesson.Date;
            currentLesson.LessonType = lesson.LessonType;
            currentLesson.Homework = lesson.Homework;
            currentLesson.NextLessonId = lesson.NextLessonId;
            currentLesson.PreviewLessonId = lesson.PreviewLessonId;
            currentLesson.SubstituteTeacherId = lesson.SubstituteTeacherId;

            currentLesson.PrepareToUpdate(_identityService);

            _db.Lessons.Update(currentLesson);
            await _db.SaveChangesAsync();

            var updatedLesson = _mapper.Map<LessonViewModel>(currentLesson);

            return Result<LessonViewModel>.SuccessWithData(updatedLesson);
        }

        private void ValidateDate(ref DateTime? fromDate, ref DateTime? toDate)
        {
            if (fromDate == null)
                fromDate = DateTime.Today;
            if (toDate == null)
                toDate = DateTime.Today.AddMonths(1);

            if (toDate < fromDate)
                toDate = fromDate;

            if (toDate.Value.Subtract(fromDate.Value) > TimeSpan.FromDays(28))
            {
                toDate = fromDate.Value.AddDays(28);
            }
        }

        private async Task<LessonViewModel> GetSubLessonAsync(long? lessonId)
        {
            if (lessonId == null)
                return null;
            var lesson = await _db.Lessons.AsNoTracking().FirstOrDefaultAsync(s => s.Id == lessonId.Value);
            if (lesson == null)
                return null;
            return _mapper.Map<LessonViewModel>(lesson);
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

            //_db.Lessons.Update(lesson);
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
            error = null;
            return true;
        }

        private bool ValidateMark(string mark)
        {
            if (string.IsNullOrEmpty(mark))
                return true;
            if (char.IsLetter(mark[0]))
            {
                var avalible = new char[] { 'н', 'Н', 'н', 'Н', 'N', 'n' };

                return avalible.Contains(mark[0]);
            }
            if (char.IsDigit(mark[0]))
            {
                var digitMark = Convert.ToInt32(mark);
                return digitMark > 0;
            }
            return false;
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
    }
}