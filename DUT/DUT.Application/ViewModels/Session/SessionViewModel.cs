using DUT.Domain.Models;
using Extensions.DeviceDetector.Models;

namespace DUT.Application.ViewModels.Session
{
    public class SessionViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public AppModel App { get; set; }
        public ClientInfo Client { get; set; }
        public Domain.Models.Location Location { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeactivatedAt { get; set; }
    }
}