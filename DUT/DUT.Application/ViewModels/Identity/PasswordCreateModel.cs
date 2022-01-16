namespace DUT.Application.ViewModels.Identity
{
    public class PasswordCreateModel : RequestModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public bool LogoutEverywhere { get; set; }
    }
}