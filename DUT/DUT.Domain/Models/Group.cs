namespace DUT.Domain.Models
{
    public class Group : BaseModel<int>
    {
        public string Name { get; set; }
        public int Course { get; set; }
        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }
        public List<UserGroup> UserGroups { get; set; }
        public List<GroupInvite> GroupInvites { get; set; }
    }

    public class GroupInvite : BaseModel<Guid>
    {
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveTo { get; set; }
        public string CodeJoin { get; set; } // Dy8Q#3478
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}