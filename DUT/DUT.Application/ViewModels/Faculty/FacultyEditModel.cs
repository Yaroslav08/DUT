using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Faculty
{
    public class FacultyEditModel : FacultyCreateModel
    {
        [Required]
        public int Id { get; set; }
    }
}
