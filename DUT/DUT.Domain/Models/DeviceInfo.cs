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

    public class OS
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Platform { get; set; }
    }

    public class Browser
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }
        public string Engine { get; set; }
        public string EngineVersion { get; set; }
    }
    public class Location
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string IP { get; set; }
    }
}
