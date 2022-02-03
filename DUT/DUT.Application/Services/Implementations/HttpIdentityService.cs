using DUT.Application.Extensions;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Identity;
using Microsoft.AspNetCore.Http;

namespace DUT.Application.Services.Implementations
{
    public class HttpIdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpContext _httpContext => _httpContextAccessor.HttpContext;
        public HttpIdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId()
        {
            return _httpContext.GetUserId();
        }

        public string GetUserName()
        {
            return _httpContext.GetUserName();
        }

        public string GetFullName()
        {
            return _httpContext.GetFullName();
        }

        public string GetLoginEmail()
        {
            return _httpContext.GetLoginEmail();
        }

        public Guid GetCurrentSessionId()
        {
            return _httpContext.GetCurrentSessionId();
        }

        public string GetIdentityData()
        {
            return _httpContext.GetIdentityData();
        }

        public string GetBearerToken()
        {
            return _httpContext.GetBearerToken();
        }

        public string GetIP()
        {
            return _httpContext.GetIP();
        }

        public IEnumerable<string> GetRoles()
        {
            return _httpContext.GetRoles();
        }

        public string GetAuthenticationMethod()
        {
            return _httpContext.GetAuthenticationMethod();
        }

        public UserIdentity GetUserDetails()
        {
            return _httpContext.GetUserDetails();
        }
    }
}