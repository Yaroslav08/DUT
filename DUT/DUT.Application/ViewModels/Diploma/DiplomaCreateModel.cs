using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Diploma
{
    public class DiplomaCreateModel : RequestModel
    {
        [Required, StringLength(5)]
        public string Series { get; set; }
        [Required]
        public int Number { get; set; }
    }
}