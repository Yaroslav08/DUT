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
        public List<QuestionModel> Result { get; set; }
    }

    public class QuestionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AnswerModel> Answers { get; set; }
    }

    public class AnswerModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool? IsCorrect { get; set; }
        public bool IsChoice { get; set; }
    }
}