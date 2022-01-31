using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Identity
{
    public class BlockUserModel
    {
        [Required]
        public int AccessFailedCount { get; set; }
        [Required]
        public bool LockoutEnabled { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public int UserId { get; set; }
    }
}