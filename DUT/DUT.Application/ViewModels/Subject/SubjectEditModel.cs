using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Subject
{
    public class SubjectEditModel : SubjectCreateModel
    {
        [Required]
        public int Id { get; set; }
    }
}