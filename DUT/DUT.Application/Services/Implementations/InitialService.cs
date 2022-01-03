using DUT.Application.Services.Interfaces;
using DUT.Constants;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class InitialService : IInitialService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly DUTDbContext _db;
        public InitialService(SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<Role> roleManager, IHttpContextAccessor httpContextAccessor, DUTDbContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }

        public async Task InitSystemAsync()
        {
            if (!await _db.Users.AnyAsync() && !await _db.Roles.AnyAsync())
            {
                #region Roles
                if (await _roleManager.FindByNameAsync(Roles.Admin) == null)
                {
                    await _roleManager.CreateAsync(new Role(Roles.Admin));
                }
                if (await _roleManager.FindByNameAsync(Roles.Moderator) == null)
                {
                    await _roleManager.CreateAsync(new Role(Roles.Moderator));
                }
                if (await _roleManager.FindByNameAsync(Roles.Teacher) == null)
                {
                    await _roleManager.CreateAsync(new Role(Roles.Teacher));
                }
                if (await _roleManager.FindByNameAsync(Roles.Student) == null)
                {
                    await _roleManager.CreateAsync(new Role(Roles.Student));
                }
                #endregion

                var user = new User("Адмін", null, "Адмін", "admin@dut.ua", "Admin");

                await _userManager.CreateAsync(user, Defaults.Password);

                await _signInManager.SignInAsync(user, true);
            }
        }
    }
}
