using Extensions.DeviceDetector.Models;
namespace DUT.Application.ViewModels.Identity
{
    public class LoginCreateModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public ClientInfo Client { get; set; }
    }
}