using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace DUT.Domain.Models
{
    public class User : IdentityUser<int>
    {
        [Required, StringLength(100, MinimumLength = 1), PersonalData]
        public string FirstName { get; set; }
        [StringLength(100, MinimumLength = 1), PersonalData]
        public string MiddleName { get; set; }
        [Required, StringLength(100, MinimumLength = 1), PersonalData]
        public string LastName { get; set; }
        [StringLength(1000, MinimumLength = 1)]
        public string Image { get; set; }
        [EmailAddress, StringLength(150, MinimumLength = 3)]
        public string ContactEmail { get; set; }
        [Phone, StringLength(15, MinimumLength = 9)]
        public string ContactPhone { get; set; }
        [Required]
        public DateTime JoinAt { get; set; }
        public List<Session> Sessions { get; set; }
        public List<UserGroup> UserGroups { get; set; }
        public List<PostComment> Comments { get; set; }
        public List<Subject> Subjects { get; set; }

        public User(string firstName, string middleName, string lastName, string email, string userName) : base(userName)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Email = email;
            JoinAt = DateTime.Now;
        }
        public User()
        {

        }
    }
}