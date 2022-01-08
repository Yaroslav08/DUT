using System.ComponentModel.DataAnnotations;
namespace DUT.Domain.Models
{
    public class PostComment : BaseModel<long>
    {
        [Required, StringLength(250, MinimumLength = 1)]
        public string Text { get; set; }
        [Required]
        public bool IsPublic { set; get; }

        public int? PostId { get; set; }
        public Post Post { get; set; }
        public int UserId { get; set; }
        public User User { set; get; }
    }
}