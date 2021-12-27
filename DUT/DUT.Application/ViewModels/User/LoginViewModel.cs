using System.ComponentModel.DataAnnotations;
namespace DUT.Application.ViewModels.User
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Login { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessage = "Пароль має бути мінімум 8 символів")]
        public string Password { get; set; }
    }
}