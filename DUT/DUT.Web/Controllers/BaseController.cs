using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace DUT.Web.Controllers
{
    [AllowAnonymous]
    public class BaseController : Controller
    {
        protected string[] GetUserRoles()
        {
            if (User.Identity.IsAuthenticated)
                return User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray();
            return null;
        }

        protected string GetFullName()
        {
            if (User.Identity.IsAuthenticated)
                return GetValueFromClaimByType("FullName");
            return null;
        }

        protected string GetFirstName()
        {
            if (User.Identity.IsAuthenticated)
                return GetValueFromClaimByType("FirstName");
            return null;
        }

        protected string GetLastName()
        {
            if (User.Identity.IsAuthenticated)
                return GetValueFromClaimByType("LastName");
            return null;
        }

        protected string GetUserName()
        {
            if (User.Identity.IsAuthenticated)
                return GetValueFromClaimByType(ClaimTypes.Name);
            return null;
        }

        protected int GetUserId()
        {
            if (User.Identity.IsAuthenticated)
                return Convert.ToInt32(GetValueFromClaimByType(ClaimTypes.NameIdentifier));
            return 0;
        }

        protected int GetSessionId()
        {
            if (User.Identity.IsAuthenticated)
                return Convert.ToInt32(GetValueFromClaimByType("SessionId"));
            return 0;
        }

        protected string GetUserAgent()
        {
            return HttpContext.Request.Headers["User-Agent"].ToString();
        }

        private string GetValueFromClaimByType(string type)
        {
            return User.Claims.FirstOrDefault(x => x.Type == type).Value;
        }

        protected string GetIP() => HttpContext.Connection.RemoteIpAddress.ToString();
    }
}