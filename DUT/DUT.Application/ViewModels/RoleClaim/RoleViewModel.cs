namespace DUT.Application.ViewModels.RoleClaim
{
    public class RoleViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public int CountClaims { get; set; }
        public List<ClaimViewModel> Claims { get; set; }
    }
}