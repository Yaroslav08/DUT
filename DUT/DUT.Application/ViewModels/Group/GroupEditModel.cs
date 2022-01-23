using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Group
{
    public class GroupEditModel : GroupCreateModel
    {
        [Required]
        public int Id { get; set; }
    }
}