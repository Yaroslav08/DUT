using DUT.Application.ViewModels.Identity;
using Microsoft.AspNetCore.Authorization;

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
    }
}
