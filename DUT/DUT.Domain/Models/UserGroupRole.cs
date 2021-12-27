namespace DUT.Domain.Models
{
    public class UserGroupRole : BaseModel<int>
    {
        public string Name { get; set; }
        public string NameEng { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string DescriptionEng { get; set; }
        public List<UserGroup> UserGroups { get; set; }
    }
}