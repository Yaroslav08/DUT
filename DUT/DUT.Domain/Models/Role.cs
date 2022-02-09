using System.ComponentModel.DataAnnotations;
namespace DUT.Domain.Models
{
    public class Role : BaseModel<int>
    {
        [Required, StringLength(150, MinimumLength = 1)]
        public string Name { get; set; }
        public List<RoleClaim> RoleClaims { get; set; }
        public int? CountClaims { get; set; }
        public string ClaimsHash { get; set; }

        public Role(string name)
        {
            Name = name;
        }
        public Role()
        {

        }
    }
}