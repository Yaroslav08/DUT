namespace DUT.Domain.Models
{
    public class UserGroupRole : BaseModel<int>
    {
        public string Name { get; set; }
        public string NameEng { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string DescriptionEng { get; set; }
        public UserGroupPermission Permissions { get; set; }
        public List<UserGroup> UserGroups { get; set; }
    }

    public class UserGroupPermission
    {
        public bool CanWriteComment { get; set; }
        public bool CanCreatePost { get; set; }
        public bool CanCreateLink { get; set; }
        public bool CanOpenCloseComment { get; set; }
        public bool CanUpdateImage { get; set; }
        public bool CanUpdateLink { get; set; }
        public bool CanRemovePost { get; set; }
        public bool CanRemoveComment { get; set; }
        public bool CanRemoveLink { get; set; }
    }
}