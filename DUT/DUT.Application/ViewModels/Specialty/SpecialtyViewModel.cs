using DUT.Application.ViewModels.Faculty;
using DUT.Application.ViewModels.Group;

namespace DUT.Application.ViewModels.Specialty
{
    public class SpecialtyViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public FacultyViewModel Faculty { get; set; }
        public List<GroupViewModel> Groups { get; set; }
    }
}