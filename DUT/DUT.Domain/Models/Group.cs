namespace DUT.Domain.Models
{
    public class Group : BaseModel<int>
    {
        public string Name { get; set; }
        public int Course { get; set; }
        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }
    }
}