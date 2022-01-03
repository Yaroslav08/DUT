using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Group
{
    public class GroupInviteCreateModel
    {
        [StringLength(75)]
        public string Name { get; set; }
        [Required]
        public DateTime ActiveFrom { get; set; }
        [Required]
        public DateTime ActiveTo { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public int GroupId { get; set; }
    }
}