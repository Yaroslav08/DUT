using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.RoleClaim
{
    public class RoleEditModel : RoleCreateModel
    {
        [Required]
        public int Id { get; set; }
    }
}