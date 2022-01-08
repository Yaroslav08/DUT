using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Faculty
{
    public class FacultyCreateModel : RequestModel
    {
        [Required, StringLength(150, MinimumLength = 1)]
        public string Name { get; set; }

        public FacultyCreateModel()
        {

        }

        public FacultyCreateModel(FacultyViewModel model)
        {
            Name = model.Name;
        }
    }
}