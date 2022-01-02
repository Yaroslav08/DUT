using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Specialty
{
    public class SpecialtyEditModel : SpecialtyCreateModel
    {
        [Required]
        public int Id { get; set; }
    }
}
