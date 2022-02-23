using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Lesson
{
    public class LessonEditModel : LessonCreateModel
    {
        [Required]
        public long Id { get; set; }
    }
}