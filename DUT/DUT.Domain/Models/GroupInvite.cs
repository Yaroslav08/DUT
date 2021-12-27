namespace DUT.Domain.Models
{
    public class GroupInvite : BaseModel<Guid>
    {
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveTo { get; set; }
        public string CodeJoin { get; set; } // Dy8Q#3478
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}