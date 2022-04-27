using System.ComponentModel.DataAnnotations;

namespace URLS.Application.ViewModels.Quiz
{
    public class AnswerEditModel : AnswerCreateModel
    {
        [Required]
        public long Id { get; set; }
    }
}