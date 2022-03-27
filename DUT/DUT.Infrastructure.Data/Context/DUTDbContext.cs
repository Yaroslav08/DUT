using DUT.Domain.Models;
using DUT.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
namespace DUT.Infrastructure.Data.Context
{
    public class DUTDbContext : DbContext
    {
        #region ctors
        public DUTDbContext(DbContextOptions<DUTDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DUTDbContext()
        {

        }
        #endregion

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleClaim> RoleClaims { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserGroupRole> UserGroupRoles { get; set; }
        public DbSet<GroupInvite> GroupInvites { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<UserSpecialty> UserSpecialties { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<App> Apps { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<Diploma> Diplomas { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<Report> Reports { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new SessionConfiguration());
            builder.ApplyConfiguration(new LessonConfiguration());
            builder.ApplyConfiguration(new SubjectConfiguration());
            builder.ApplyConfiguration(new UserGroupRoleConfiguration());
            builder.ApplyConfiguration(new PostCommentConfiguration());
            builder.ApplyConfiguration(new LoginAttemptConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new SettingConfiguration());
            builder.ApplyConfiguration(new TimetableConfiguration());
            builder.ApplyConfiguration(new ReportConfiguration());
        }
    }
}