namespace DUT.Application.Services.Interfaces
{
    public interface IIdentityService
    {
        int GetUserId();
        string GetUserName();
        string GetFullName();
        string GetLoginEmail();
        int GetCurrentSessionId();
        string GetIdentityData();
        string GetBearerToken();
        string GetIP();
        string GetRole();
    }
}