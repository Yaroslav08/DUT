using DUT.Application.Services.Interfaces;
using DUT.Constants;

namespace DUT.Application.Services.Implementations
{
    public class FakeIdentityService : IIdentityService
    {
        public int GetCurrentSessionId()
        {
            return 1;
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

        public string GetBearerToken()
        {
            return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
        }

        public string GetIP()
        {
            return "34.43.119.37";
        }

        public string GetRole()
        {
            return Roles.Admin;
        }

        public string GetAuthenticationMethod()
        {
            return "pwd";
        }
    }
}