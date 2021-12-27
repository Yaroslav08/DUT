using System.ComponentModel.DataAnnotations;
namespace DUT.Domain.Models
{
    public class University : BaseModel<int>
    {
        [Required]
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string NameEng { get; set; }
        public string ShortNameEng { get; set; }
        public List<Faculty> Faculties { get; set; }
    }
}