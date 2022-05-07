using System.ComponentModel.DataAnnotations;
namespace URLS.Application.ViewModels.Firebase
{
    public class SubscribeModel
    {
        [Required]
        public string Token { get; set; }
        public int Type { get; set; }
    }
}