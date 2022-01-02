using System.ComponentModel.DataAnnotations;

namespace DUT.Domain.Models
{
    public class Session : BaseModel<int>
    {
        [Required, StringLength(150, MinimumLength = 1)]
        public string App { get; set; } // "Dut Android 1.0"
        public DeviceInfo Device { get; set; }
        public Location Location { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public int DeactivatedBySessionId { get; set; }
        [StringLength(5000, MinimumLength = 5)]
        public string Token { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}