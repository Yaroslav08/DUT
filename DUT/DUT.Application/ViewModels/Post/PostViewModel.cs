using DUT.Application.ViewModels.Group;
using DUT.Application.ViewModels.Post.Comment;
using DUT.Application.ViewModels.User;
namespace DUT.Application.ViewModels.Post
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsImportant { get; set; }
        public bool AvailableToComment { get; set; }
        public bool IsPublic { get; set; }
        public int CountComments { get; set; }
        public UserViewModel User { get; set; }
        public GroupViewModel Group { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }
}