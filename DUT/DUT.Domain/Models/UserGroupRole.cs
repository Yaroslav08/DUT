namespace DUT.Domain.Models
{
    public class UserGroupRole : BaseModel<int>
    {
        public string Name { get; set; }
        public string NameEng { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string DescriptionEng { get; set; }
        public bool CanEdit { get; set; }
        public string UniqId { get; set; }
        public UserGroupPermission Permissions { get; set; }
        public List<UserGroup> UserGroups { get; set; }
    }

    public class UserGroupPermission
    {
        public bool CanCreatePost { get; set; }
        public bool CanEditPost { get; set; }
        public bool CanEditAllPosts { get; set; }
        public bool CanRemovePost { get; set; }
        public bool CanRemoveAllPosts { get; set; }

        public bool CanWriteComment { get; set; }
        public bool CanOpenCloseComment { get; set; }
        public bool CanRemoveComment { get; set; }
        public bool CanRemoveAllComments { get; set; }

        public bool CanCreateInviteCode { get; set; }
        public bool CanUpdateInviteCode { get; set; }
        public bool CanRemoveInviteCode { get; set; }


        public bool CanUpdateImage { get; set; }
        public bool CanEditInfo { get; set; }
    }
}