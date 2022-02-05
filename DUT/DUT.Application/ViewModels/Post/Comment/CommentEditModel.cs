using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Post.Comment
{
    public class CommentEditModel : CommentCreateModel
    {
        [Required]
        public long Id { get; set; }
    }
}