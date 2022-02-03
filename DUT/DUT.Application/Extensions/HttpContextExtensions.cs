using DUT.Application.ViewModels.Identity;
using DUT.Constants;
using Microsoft.AspNetCore.Http;
namespace DUT.Application.Extensions
{
    public static class HttpContextExtensions
    {
        public static int GetUserId(this HttpContext _httpContext)
        {
            var claim = _httpContext.User.Claims.FirstOrDefault(s => s.Type == CustomClaimTypes.UserId);
            return Convert.ToInt32(claim.Value);
        }

        public static string GetUserName(this HttpContext _httpContext)
        {
            var claim = _httpContext.User.Claims.FirstOrDefault(s => s.Type == CustomClaimTypes.UserName);
            return claim.Value;
        }

        public static string GetFullName(this HttpContext _httpContext)
        {
            var claim = _httpContext.User.Claims.FirstOrDefault(s => s.Type == CustomClaimTypes.FullName);
            return claim.Value;
        }

        public static string GetLoginEmail(this HttpContext _httpContext)
        {
            var claim = _httpContext.User.Claims.FirstOrDefault(s => s.Type == CustomClaimTypes.Login);
            return claim.Value;
        }

        public static Guid GetCurrentSessionId(this HttpContext _httpContext)
        {
            var claim = _httpContext.User.Claims.FirstOrDefault(s => s.Type == CustomClaimTypes.CurrentSessionId);
            return Guid.Parse(claim.Value);
        }

        public static string GetIdentityData(this HttpContext _httpContext)
        {
            return $"{GetLoginEmail(_httpContext)} ({GetUserId(_httpContext)})";
        }

        public static string GetBearerToken(this HttpContext _httpContext)
        {
            var bearerWord = "Bearer ";
            var bearerToken = _httpContext.Request.Headers["Authorization"].ToString();
            if (bearerToken.StartsWith(bearerWord, StringComparison.OrdinalIgnoreCase))
            {
                return bearerToken.Substring(bearerWord.Length).Trim();
            }
            return bearerToken;
        }

        public static string GetIP(this HttpContext _httpContext)
        {
            return _httpContext.Connection.RemoteIpAddress.ToString();
        }

        public static IEnumerable<string> GetRoles(this HttpContext _httpContext)
        {
            var claim = _httpContext.User.Claims.Where(s => s.Type == CustomClaimTypes.Role);
            return claim.Select(x => x.Value);
        }
        public static string GetAuthenticationMethod(this HttpContext _httpContext)
        {
            var claim = _httpContext.User.Claims.FirstOrDefault(s => s.Type == CustomClaimTypes.AuthenticationMethod);
            return claim.Value;
        }

        public static UserIdentity GetUserDetails(this HttpContext _httpContext)
        {
            var claims = _httpContext.User.Claims;
            var roles = claims.Where(s => s.Type == CustomClaimTypes.Role);
            return new UserIdentity
            {
                CurrentSessionId = GetCurrentSessionId(_httpContext),
                Fullname = GetFullName(_httpContext),
                Id = GetUserId(_httpContext),
                Login = GetLoginEmail(_httpContext),
                Username = GetUserName(_httpContext),
                Claims = claims,
                Roles = roles.Select(s => s.Value),
            };
        }
    }
}