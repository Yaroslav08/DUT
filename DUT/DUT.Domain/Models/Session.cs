namespace DUT.Domain.Models
{
    public class Session : BaseModel<int>
    {
        public string App { get; set; } // "Dut Android 1.0"
        public DeviceInfo Device { get; set; }
        public Location Location { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public int DeactivatedBySessionId { get; set; }
        public string Token { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}