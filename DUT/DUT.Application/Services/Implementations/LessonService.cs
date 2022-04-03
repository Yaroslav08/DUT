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

            var subject = _subjectService.Exists.First();

            if (lesson.LessonType == LessonType.Exam && !subject.Config.WithExam)
                return Result<LessonViewModel>.Error("Exam lesson can't be create due to config");

            if (subject.IsTemplate || subject.GroupId == null)
                return Result<LessonViewModel>.Error("Can't create lesson due to this subject is template");

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
    }
}