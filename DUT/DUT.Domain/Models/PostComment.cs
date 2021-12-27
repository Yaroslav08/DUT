namespace DUT.Domain.Models
{
    public class PostComment : BaseModel<long>
    {
        public string Text { get; set; }
        public bool IsPublic { set; get; }

        public int PostId { get; set; }
        public Post Post { get; set; }
        public int UserId { get; set; }
        public User User { set; get; }
    }
}