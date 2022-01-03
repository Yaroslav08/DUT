using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Group
{
    public class GroupInviteEditModel : GroupInviteCreateModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}