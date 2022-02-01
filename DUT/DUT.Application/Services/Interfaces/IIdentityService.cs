using DUT.Application.ViewModels.Identity;

namespace DUT.Application.Services.Interfaces
{
    public interface IIdentityService
    {
        int GetUserId();
        string GetUserName();
        string GetFullName();
        string GetLoginEmail();
        Guid GetCurrentSessionId();
        string GetIdentityData();
        string GetBearerToken();
        string GetIP();
        IEnumerable<string> GetRoles();
        string GetAuthenticationMethod();
        UserIdentity GetUserDetails();
    }
}