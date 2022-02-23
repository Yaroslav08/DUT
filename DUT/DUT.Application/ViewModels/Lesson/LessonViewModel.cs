using DUT.Application.ViewModels.Subject;
using DUT.Domain.Models;

namespace DUT.Application.ViewModels.Lesson
{
    public class LessonViewModel
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Theme { get; set; }
        public string Description { get; set; }
        public Journal Journal { get; set; }
        public DateTime Date { get; set; }
        public LessonType LessonType { get; set; }
        public string Homework { get; set; }
        public LessonViewModel PreviewLesson { get; set; }
        public LessonViewModel NextLesson { get; set; }
        public SubjectViewModel Subject { get; set; }
    }
}