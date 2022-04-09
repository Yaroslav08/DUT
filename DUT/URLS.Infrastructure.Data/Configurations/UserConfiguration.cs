using URLS.Domain.Models;
using URLS.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace URLS.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.NotificationSettings).HasConversion(
                x => x.ToJson(),
                s => s.FromJson<NotificationSettings>());
        }
    }
}