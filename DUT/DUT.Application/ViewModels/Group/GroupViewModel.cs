using DUT.Application.ViewModels.Specialty;
namespace DUT.Application.ViewModels.Group
{
    public class GroupViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Course { get; set; }
        public int CountOfStudents { get; set; }
        public SpecialtyViewModel Specialty { get; set; }
    }
}