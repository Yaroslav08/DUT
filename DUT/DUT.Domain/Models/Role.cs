using System.ComponentModel.DataAnnotations;
namespace DUT.Domain.Models
{
    public class Role : BaseModel<int>
    {
        [Required]
        public string Name { get; set; }
        public List<RoleClaim> RoleClaims { get; set; }
    }
}