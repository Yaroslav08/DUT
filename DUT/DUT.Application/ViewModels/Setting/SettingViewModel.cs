namespace DUT.Application.ViewModels.Setting
{
    public class SettingViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime FirtsSemesterStart { get; set; }
        public DateTime FirtsSemesterEnd { get; set; }
        public DateTime SecondSemesterStart { get; set; }
        public DateTime SecondSemesterEnd { get; set; }
        public int MaxCourseInUniversity { get; set; }
    }
}
