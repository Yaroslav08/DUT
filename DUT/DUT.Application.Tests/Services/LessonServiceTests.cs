using AutoMapper;
using DUT.Application.Services.Implementations;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Lesson;
using DUT.Domain.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DUT.Application.Tests.Services
{
    public class LessonServiceTests
    {
        [Fact]
        public async void UpdateJournalWithValidData()
        {
            var dbContext = DUTDbContextFactory.CreateDbContext();

            dbContext.Lessons.Add(new Lesson
            {
                Id = 1,
                Theme = "d",
                Date = DateTime.Today,
                LessonType = LessonType.Practical,
                SubjectId = 1,
                Journal = new Journal
                {
                    Students = new List<Student>
                    {
                        new Student
                        {
                            Id = 1,
                            Name = "Один Одинович",
                            Mark = null
                        },
                        new Student
                        {
                            Id = 2,
                            Name = "Два Двач",
                            Mark = null
                        },
                        new Student
                        {
                            Id = 3,
                            Name = "Три трич",
                            Mark = null
                        },
                        new Student
                        {
                            Id = 4,
                            Name = "Чотири Чвач",
                            Mark = null
                        },
                        new Student
                        {
                            Id = 5,
                            Name = "П'ять Пятч",
                            Mark = null
                        }
                    }
                }
            });
            dbContext.Subjects.Add(new Subject
            {
                GroupId = 1,
                Id = 1,
                Name = "",
                Semestr = 1,
                From = DateTime.Today,
                To = DateTime.Now,
                IsTemplate = false,
            });
            dbContext.SaveChanges();

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(s => s.Map<LessonViewModel>(It.IsAny<Lesson>()))
                .Returns((Lesson src) => new LessonViewModel()
                {
                    Id = src.Id,
                    CreatedAt = src.CreatedAt,
                    Description = src.Description,
                    Homework = src.Homework,
                    Journal = src.Journal,
                    Date = src.Date,
                    IsSubstitute = src.SubstituteTeacherId == null,
                    LessonType = src.LessonType,
                    Theme = src.Theme
                });

            var newJournal = new Journal
            {
                Students = new List<Student>
                    {
                        new Student
                        {
                            Id = 1,
                            Name = "Один Одинович",
                            Mark = "5"
                        },
                        new Student
                        {
                            Id = 2,
                            Name = "Два Двач",
                            Mark = "1"
                        },
                        new Student
                        {
                            Id = 3,
                            Name = "Три трич",
                            Mark = null
                        },
                        new Student
                        {
                            Id = 4,
                            Name = "Чотири Чвач",
                            Mark = "12"
                        },
                        new Student
                        {
                            Id = 5,
                            Name = "П'ять Пятч",
                            Mark = "н"
                        }
                    }
            };

            var lessonService = new LessonService(null, null, mapperMock.Object, dbContext, null);

            var res = await lessonService.UpdateJournalAsync(1, 1, newJournal);

            Assert.True(res.IsSuccess);
            Assert.NotNull(res.Data);
            Assert.Equal(5, res.Data.Journal.Students.Count);
        }

        [Fact]
        public async void UpdateJournalWithIncorrectData()
        {
            var dbContext = DUTDbContextFactory.CreateDbContext();

            dbContext.Lessons.Add(new Lesson
            {
                Id = 1,
                Theme = "d",
                Date = DateTime.Today,
                LessonType = LessonType.Practical,
                SubjectId = 1,
                Journal = new Journal
                {
                    Students = new List<Student>
                    {
                        new Student
                        {
                            Id = 1,
                            Name = "Один Одинович",
                            Mark = null
                        },
                        new Student
                        {
                            Id = 2,
                            Name = "Два Двач",
                            Mark = null
                        },
                        new Student
                        {
                            Id = 3,
                            Name = "Три трич",
                            Mark = null
                        },
                        new Student
                        {
                            Id = 4,
                            Name = "Чотири Чвач",
                            Mark = null
                        },
                        new Student
                        {
                            Id = 5,
                            Name = "П'ять Пятч",
                            Mark = null
                        }
                    }
                }
            });
            dbContext.Subjects.Add(new Subject
            {
                GroupId = 1,
                Id = 1,
                Name = "",
                Semestr = 1,
                From = DateTime.Today,
                To = DateTime.Now,
                IsTemplate = false,
            });
            dbContext.SaveChanges();

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(s => s.Map<LessonViewModel>(It.IsAny<Lesson>()))
                .Returns((Lesson src) => new LessonViewModel()
                {
                    Id = src.Id,
                    CreatedAt = src.CreatedAt,
                    Description = src.Description,
                    Homework = src.Homework,
                    Journal = src.Journal,
                    Date = src.Date,
                    IsSubstitute = src.SubstituteTeacherId == null,
                    LessonType = src.LessonType,
                    Theme = src.Theme
                });

            var newJournal = new Journal
            {
                Students = new List<Student>
                    {
                        new Student
                        {
                            Id = 1,
                            Name = "Один Одинович",
                            Mark = "5"
                        },
                        new Student
                        {
                            Id = 2,
                            Name = "Два Двач",
                            Mark = "1"
                        },
                        new Student
                        {
                            Id = 3,
                            Name = "Три трич",
                            Mark = null
                        },
                        new Student
                        {
                            Id = 4,
                            Name = "Чотири Чвач",
                            Mark = "12"
                        },
                        new Student
                        {
                            Id = 7,
                            Name = "П'ять Пятч",
                            Mark = "d"
                        }
                    }
            };

            var lessonService = new LessonService(null, null, mapperMock.Object, dbContext, null);

            var res = await lessonService.UpdateJournalAsync(1, 1, newJournal);

            Assert.True(res.IsError);
            Assert.Equal("Студент П'ять Пятч не існує", res.ErrorMessage);
        }
    }
}
