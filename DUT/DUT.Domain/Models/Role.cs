namespace DUT.Domain.Models
{
    public class Role : IdentityRole<int, UserRole>
    {
        public Role() { }
        public Role(string name) { Name = name; }
    }
}