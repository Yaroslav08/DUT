namespace DUT.Domain.Models
{
    public class SubjectConfig
    {
        public int RecommendedForCourse { get; set; }
        public int MaxMarkPerLesson { get; set; }
        public int MinMarkPerLesson { get; set;}
        public bool WithExam { get; set; }
        public int MaxMark { get; set; }
        public int MaxMarkUpToExam { get; set; }
        public int MaxMarkInExam { get; set; }
        public int CommonHours { get; set; }
        public int LectureHours { get; set; }
        public int PracticHours { get; set; }
        public int LabsHours { get; set; }
        public int IndividualHours { get; set; }
        public int ExamHours { get; set; }
        public int OffsetHours { get; set; }
    }
}
