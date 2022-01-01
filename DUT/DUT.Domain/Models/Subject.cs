namespace DUT.Domain.Models
{
    public class Subject : BaseModel<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Semestr { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool IsTemplate { get; set; }
        public SubjectConfig Config { get; set; }

        public int? GroupId { get; set; }
        public Group Group { get; set; }

        public int TeacherId { get; set; }
        public User Teacher { get; set; }
        public List<Lesson> Lessons { get; set; }
    }
}
