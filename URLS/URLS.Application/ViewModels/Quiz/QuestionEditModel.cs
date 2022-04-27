using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Quiz
{
    public class QuestionEditModel : QuestionCreateModel
    {
        [Required]
        public int Id { get; set; }
    }
}