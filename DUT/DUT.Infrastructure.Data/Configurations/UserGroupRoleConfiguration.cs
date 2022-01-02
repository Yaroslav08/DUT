using DUT.Domain.Models;
using DUT.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DUT.Infrastructure.Data.Configurations
{
    public class UserGroupRoleConfiguration : IEntityTypeConfiguration<UserGroupRole>
    {
        public void Configure(EntityTypeBuilder<UserGroupRole> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Permissions).HasConversion(
                v => v.ToJson(),
                v => v.FromJson<UserGroupPermission>());
        }
    }
}