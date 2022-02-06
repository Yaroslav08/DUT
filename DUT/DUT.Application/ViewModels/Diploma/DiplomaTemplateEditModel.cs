using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Diploma
{
    public class DiplomaTemplateEditModel : DiplomaTemplateCreateModel
    {
       [Required]
        public string Id { get; set; }
    }
}