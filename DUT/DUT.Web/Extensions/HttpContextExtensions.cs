using DUT.Application.ViewModels.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DUT.Web.Extensions
{
    public static class HttpContextExtensions
    {
        public static bool IsAuthenticationRequired(this HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            var metadata = endpoint.Metadata;

            var existAuthAttrbt = metadata.Any(s => s is AuthorizeAttribute);
            if (existAuthAttrbt)
            {
                if (metadata.Any(s => s is AllowAnonymousAttribute))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public static bool HasEmail(this ClaimsPrincipal user, IEnumerable<string> emails)
        {
            if (!user.Identity.IsAuthenticated)
                return false;
            return user.Claims.Any(s => s.Type == ClaimTypes.Email && emails.Contains(s.Value));
        }
    }
}
