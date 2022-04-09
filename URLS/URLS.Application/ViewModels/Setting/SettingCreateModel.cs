using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Setting
{
    public class SettingCreateModel
    {
        [Required]
        public DateTime FirtsSemesterStart { get; set; }
        [Required]
        public DateTime FirtsSemesterEnd { get; set; }
        [Required]
        public DateTime SecondSemesterStart { get; set; }
        [Required]
        public DateTime SecondSemesterEnd { get; set; }
        [Required]
        public int MaxCourseInUniversity { get; set; }
    }
}
