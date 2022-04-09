using System.Security.Claims;
namespace URLS.Application.ViewModels.Identity
{
    public class JwtToken
    {
        public string Token { get; set; }
        public string TokenId { get; set; }
        public DateTime ExpiredAt { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
    }
}