namespace DUT.Domain.Models
{
    public class UserSpecialty : BaseModel<int>
    {
        public string Title { get; set; }
        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}