using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace URLS.Application.ViewModels.User
{
    public class RegisterViewModel
    {
        [Required, StringLength(100, MinimumLength = 1)]
        public string FirstName { get; set; }
        [StringLength(100, MinimumLength = 1)]
        public string MiddleName { get; set; }
        [Required, StringLength(100, MinimumLength = 1)]
        public string LastName { get; set; }
        [Required, EmailAddress, StringLength(200, MinimumLength = 5)]
        public string Login { get; set; }
        [Display(Name = "Пароль:")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$")]
        public string Password { get; set; }
        [Required, StringLength(9, MinimumLength = 9)]
        [Display(Name = "Код групи:")]
        public string Code { get; set; }
    }
}