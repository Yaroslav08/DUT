namespace DUT.Application.Services.Interfaces
{
    public interface IIdentityService
    {
        int GetUserId();
        string GetUserName();
        string GetFullName();
        string GetLoginEmail();
        string GetCurrentSessionId();
        string GetIdentityData();
    }
}