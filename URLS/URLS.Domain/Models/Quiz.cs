using System.ComponentModel.DataAnnotations;
namespace URLS.Domain.Models
{

    public class Quiz : BaseModel<int>
    {
        [Required, StringLength(150, MinimumLength = 2)]
        public string Name { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
        public int MaxAttempts { get; set; } // (-1) without limits
        public double MarkPerQuiz { get; set; }
        public bool IsTemplate { get; set; }
        public int? SubjectId { get; set; }
        public Subject Subject { get; set; }
        public List<Question> Questions { get; set; }
        public List<QuizResult> QuizResults { get; set; }
    }
}