using DUT.Application.Services.Interfaces;

namespace DUT.Application.Services.Implementations
{
    public class FakeIdentityService : IIdentityService
    {
        public string GetCurrentSessionId()
        {
            return "1";
        }

        public string GetIdentityData()
        {
            return $"{GetLoginEmail()} ({GetUserId()})";
        }

        public string GetFullName()
        {
            return "Мудрик Ярослав";
        }

        public string GetLoginEmail()
        {
            return "yaroslav.mudryk@gmail.com";
        }

        public int GetUserId()
        {
            return 1;
        }

        public string GetUserName()
        {
            return "Yarik08";
        }
    }
}