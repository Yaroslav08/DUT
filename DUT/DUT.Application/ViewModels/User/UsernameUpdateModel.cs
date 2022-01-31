using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.User
{
    public class UsernameUpdateModel
    {
        public int? UserId { get; set; }
        [Required, StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }
    }
}