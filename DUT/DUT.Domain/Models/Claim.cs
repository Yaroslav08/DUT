namespace DUT.Domain.Models
{
    public class Claim : BaseModel<int>
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public List<RoleClaim> RoleClaims { get; set; }
    }
}