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
        public DateTime JoinAt { get; set; }
        public List<Session> Sessions { get; set; }
        public List<UserGroup> UserGroups { get; set; }

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