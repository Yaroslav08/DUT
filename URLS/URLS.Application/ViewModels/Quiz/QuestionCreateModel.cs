using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Quiz
{
    public class QuestionCreateModel
    {
        [Required, StringLength(500, MinimumLength = 1)]
        public string Name { get; set; }
        [Required]
        public int Index { get; set; }
        public List<AnswerCreateModel> Answers { get; set; }
    }
}