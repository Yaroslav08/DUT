using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Identity;
using DUT.Constants;
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
            var claim = _httpContext.User.Claims.FirstOrDefault(s => s.Type == CustomClaimTypes.UserId);
            return Convert.ToInt32(claim.Value);
        }

        public string GetUserName()
        {
            var claim = _httpContext.User.Claims.FirstOrDefault(s => s.Type == CustomClaimTypes.UserName);
            return claim.Value;
        }

        public string GetFullName()
        {
            var claim = _httpContext.User.Claims.FirstOrDefault(s => s.Type == CustomClaimTypes.FullName);
            return claim.Value;
        }

        public string GetLoginEmail()
        {
            var claim = _httpContext.User.Claims.FirstOrDefault(s => s.Type == CustomClaimTypes.Login);
            return claim.Value;
        }

        public Guid GetCurrentSessionId()
        {
            var claim = _httpContext.User.Claims.FirstOrDefault(s => s.Type == CustomClaimTypes.CurrentSessionId);
            return Guid.Parse(claim.Value);
        }

        public string GetIdentityData()
        {
            return $"{GetLoginEmail()} ({GetUserId()})";
        }

        public string GetBearerToken()
        {
            var bearerWord = "Bearer ";
            var bearerToken = _httpContext.Request.Headers["Authorization"].ToString();
            if (bearerToken.StartsWith(bearerWord, StringComparison.OrdinalIgnoreCase))
            {
                return bearerToken.Substring(bearerWord.Length).Trim();
            }
            return bearerToken;
        }

        public string GetIP()
        {
            return _httpContext.Connection.RemoteIpAddress.ToString();
        }

        public string GetRole()
        {
            var claim = _httpContext.User.Claims.FirstOrDefault(s => s.Type == CustomClaimTypes.Role);
            return claim.Value;
        }

        public string GetAuthenticationMethod()
        {
            var claim = _httpContext.User.Claims.FirstOrDefault(s => s.Type == CustomClaimTypes.AuthenticationMethod);
            return claim.Value;
        }

        public UserIdentity GetUserDetails()
        {
            var claims = _httpContext.User.Claims;
            var roles = claims.Where(s => s.Type == CustomClaimTypes.Role);
            return new UserIdentity
            {
                CurrentSessionId = GetCurrentSessionId(),
                Fullname = GetFullName(),
                Id = GetUserId(),
                Login = GetLoginEmail(),
                Username = GetUserName(),
                Claims = claims,
                Roles = roles.Select(s => s.Value),
            };
        }
    }
}