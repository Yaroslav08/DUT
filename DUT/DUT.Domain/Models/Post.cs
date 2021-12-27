namespace DUT.Domain.Models
{
    public class Post : BaseModel<int>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsImportant { get; set; }
        public bool AvailableToComment { get; set; }
        public bool IsPublic { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public List<PostComment> Comments { get; set; }
    }
}