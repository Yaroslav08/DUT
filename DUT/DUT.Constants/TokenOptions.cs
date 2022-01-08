using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace DUT.Constants
{
    public class TokenOptions
    {
        public const string Issuer = "DUT ID";
        public const string Audience = "DUT Client";
        const string SecretKey = "418b66dc05744f90b3fdc246ecc7dd372ca30e632f1744649f1df0cef16e2222";
        public const int LifeTimeInDays = 45;
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
    }
}