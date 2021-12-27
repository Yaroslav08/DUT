using System.ComponentModel.DataAnnotations;
namespace DUT.Application.ViewModels.User
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
    }
}