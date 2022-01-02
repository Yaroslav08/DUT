namespace DUT.Domain.Models
{
    public class DeviceInfo
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string BrandShortName { get; set; }
        public OS OS { get; set; }
        public Browser Browser { get; set; }
    }
}