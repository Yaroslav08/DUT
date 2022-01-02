using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.University
{
    public class UniversityEditModel : UniversityCreateModel
    {
        [Required]
        public int Id { get; set; }
    }
}
