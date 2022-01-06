using DUT.Domain.Models;
using DUT.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DUT.Infrastructure.Data.Context
{
    public class DUTDbContext : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        #region ctors
        public DUTDbContext(DbContextOptions<DUTDbContext> options) : base(options)
        {

        }
        public DUTDbContext()
        {

        }
        #endregion

        public DbSet<Session> Sessions { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserGroupRole> UserUserGroupRoles { get; set; }
        public DbSet<GroupInvite> GroupInvites { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<UserSpecialty> UserSpecialties { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<App> Apps { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new SessionConfiguration());
            builder.ApplyConfiguration(new LessonConfiguration());
            builder.ApplyConfiguration(new SubjectConfiguration());
            builder.ApplyConfiguration(new UserGroupRoleConfiguration());
        }
    }
}
