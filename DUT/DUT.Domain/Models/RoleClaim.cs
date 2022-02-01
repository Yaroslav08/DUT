using System.ComponentModel.DataAnnotations;

namespace DUT.Domain.Models
{
    public class RoleClaim : BaseModel<int>
    {
        [Required, StringLength(500)]
        public string Type { get; set; }
        [Required, StringLength(500)]
        public string Value { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}