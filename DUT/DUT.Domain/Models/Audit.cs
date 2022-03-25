using System.ComponentModel.DataAnnotations;

namespace DUT.Domain.Models
{
    public class Audit : BaseModel<long>
    {
        [Required]
        public string EntityId { get; set; }
        [Required]
        public string Entity { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
    }
}