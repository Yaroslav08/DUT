using Microsoft.AspNetCore.Authorization;

namespace DUT.Web.Extensions
{
    public static class HttpContextExtensions
    {
        public static bool IsAuthenticationRequired(this HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            var metadata = endpoint.Metadata;
            foreach (var item in metadata)
            {
                if (item is AuthorizeAttribute)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
