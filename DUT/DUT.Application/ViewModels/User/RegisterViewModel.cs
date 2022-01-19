using System.ComponentModel.DataAnnotations;
namespace DUT.Application.ViewModels.User
{
    public class RegisterViewModel : RequestModel
    {
        [Display(Name = "Ім'я:")]
        public string FirstName { get; set; }
        [Display(Name = "Прізвище:")]
        public string LastName { get; set; }
        [Display(Name = "Пошта:")]
        public string Login { get; set; }
        [Display(Name = "Пароль:")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$")]
        public string Password { get; set; }
        [Required, StringLength(9, MinimumLength = 9)]
        [Display(Name = "Код групи:")]
        public string Code { get; set; }
    }
}