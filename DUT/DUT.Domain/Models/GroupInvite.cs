using System.ComponentModel.DataAnnotations;

namespace DUT.Domain.Models
{
    public class GroupInvite : BaseModel<Guid>
    {
        [Required]
        public DateTime ActiveFrom { get; set; }
        [Required]
        public DateTime ActiveTo { get; set; }
        [Required, StringLength(10, MinimumLength = 8)]
        public string CodeJoin { get; set; } // Dy8Q#3478
        [Required]
        public bool IsActive { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}