using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Post
{
    public class PostEditModel : PostCreateModel
    {
        [Required]
        public int Id { get; set; }
    }
}
