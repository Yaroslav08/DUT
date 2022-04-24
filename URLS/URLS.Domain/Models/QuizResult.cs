namespace URLS.Domain.Models
{
    public class QuizResult : BaseModel<int>
    {
        public double Mark { get; set; }
        public int Attempt { get; set; }
        public QuizResultStatistics Statistics { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public Guid QuizId { get; set; }
        public Quiz Quiz { get; set; }
        public List<QuizAnswer> Answers { get; set; }
    }

    public class QuizAnswer
    {

    }
}