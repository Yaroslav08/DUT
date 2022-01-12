using DUT.Constants.APIResponse;
using System.Net;

namespace DUT.Web.Middlewares
{
    public class GlobalErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public GlobalErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
                return;
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            var typeAppRequest = GetTypeApp(httpContext);
            if (typeAppRequest == "mvc")
            {
                httpContext.Response.Redirect("/Home/Error");
            }
            else
            {
                var requestId = httpContext.TraceIdentifier;
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(new APIResponse(false, "Internal server error", exception.Message, requestId));
            }
        }

        private string GetTypeApp(HttpContext httpContext)
        {
            return "api";
            var enpoint = httpContext.Request.Path.Value;
            if (enpoint.StartsWith("/api/v"))
                return "api";
            return "mvc";
        }
    }

    public static class GlobalErrorHandlerMiddlewareExtensions
    {
        public static void UseGlobalErrorHandler(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<GlobalErrorHandlerMiddleware>();
        }
    }
}
