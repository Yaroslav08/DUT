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
        public List<Post> Posts { get; set; }
        public List<Subject> Subjects { get; set; }
    }
}