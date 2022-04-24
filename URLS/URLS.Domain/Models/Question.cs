using System.ComponentModel.DataAnnotations;
namespace URLS.Domain.Models
{
    public class Question : BaseModel<int>
    {
        [Required, StringLength(500, MinimumLength = 1)]
        public string Name { get; set; }
        [Required]
        public int Index { get; set; }
        public long? CorrectAnswerId { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
        public List<Answer> Answers { get; set; }
    }
}