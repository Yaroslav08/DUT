using Microsoft.AspNetCore.Identity;
namespace DUT.Domain.Models
{
    public class Role : IdentityRole<int>
    {
        public Role(string name) : base(name) { }
    }
}