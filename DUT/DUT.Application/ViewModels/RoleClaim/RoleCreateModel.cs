using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.RoleClaim
{
    public class RoleCreateModel
    {
        [Required, StringLength(150, MinimumLength = 1)]
        public string Name { get; set; }
        public int[] ClaimsIds { get; set; }
    }
}