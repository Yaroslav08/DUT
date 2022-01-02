using DUT.Application.ViewModels.Specialty;
using DUT.Application.ViewModels.University;

namespace DUT.Application.ViewModels.Faculty
{
    public class FacultyViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public UniversityViewModel University { get; set; }
        public List<SpecialtyViewModel> Specialties { get; set; }
    }
}