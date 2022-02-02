using DUT.Constants;
using Extensions.DeviceDetector.Models;
using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Identity
{
    public class LoginCreateModel : RequestModel
    {
        [Required, EmailAddress, StringLength(200, MinimumLength = 5)]
        public string Login { get; set; }
        [Required]
        [RegularExpression(RegexTemplate.Password.Regex, ErrorMessage = RegexTemplate.Password.ErrorMessage)]
        public string Password { get; set; }
        [Required, StringLength(30)]
        public string AppId { get; set; }
        [Required, StringLength(70)]
        public string AppSecret { get; set; }
        public ClientInfo Client { get; set; }
    }
}