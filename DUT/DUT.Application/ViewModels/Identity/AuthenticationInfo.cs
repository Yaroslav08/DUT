namespace DUT.Application.ViewModels.Identity
{
    public class AuthenticationInfo
    {
        public Domain.Models.User User { get; set; }
        public Domain.Models.Session Session { get; set; }
    }
}