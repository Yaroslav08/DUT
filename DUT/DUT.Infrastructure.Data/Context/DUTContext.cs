using DUT.Domain.Models;
using DUT.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DUT.Infrastructure.Data.Context
{
    public class DUTContext : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        #region ctors
        public DUTContext(DbContextOptions<DUTContext> options) : base(options)
        {

        }
        public DUTContext()
        {

        }
        #endregion

        public DbSet<Session> Sessions { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Group> Groups { get; set; }





        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new SessionConfiguration());
        }
    }
}
