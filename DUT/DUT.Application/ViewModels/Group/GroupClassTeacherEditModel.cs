using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Group
{
    public class GroupClassTeacherEditModel
    {
        public int? GroupId { get; set; }
        [StringLength(50)]
        public string Title { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}