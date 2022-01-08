using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Faculty
{
    public class FacultyCreateModel : RequestModel
    {
        [Required, StringLength(150, MinimumLength = 1)]
        public string Name { get; set; }
    }
}
