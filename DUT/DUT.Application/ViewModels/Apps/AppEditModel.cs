using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Apps
{
    public class AppEditModel : AppCreateModel
    {
        [Required]
        public int Id { get; set; }
    }
}
