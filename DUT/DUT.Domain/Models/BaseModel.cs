using System.ComponentModel.DataAnnotations;
namespace DUT.Domain.Models
{
    public class BaseModel
    {
        [Required]
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedFromIP { get; set; }

        public DateTime? LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
        public string LastUpdatedFromIP { get; set; }
    }
    public class BaseModel<T> : BaseModel
    {
        [Key]
        public T Id { get; set; }
    }
}