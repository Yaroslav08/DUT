using System.ComponentModel.DataAnnotations;

namespace URLS.Domain.Models
{
    public class UserLogin : BaseModel<int>
    {
        [Required, StringLength(100)]
        public string ExternalProvider { get; set; }
        [StringLength(150)]
        public string Email { get; set; }
        [StringLength(150)]
        public string Key { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}