using DUT.Domain.Models;
using DUT.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DUT.Infrastructure.Data.Configurations
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Device).HasConversion(
                v => v.ToJson(),
                v => v.FromJson<DeviceInfo>());
            builder.Property(x => x.Location).HasConversion(
                v => v.ToJson(),
                v => v.FromJson<Location>());
            builder.Property(x => x.App).HasConversion(
                v => v.ToJson(),
                v => v.FromJson<AppModel>());
        }
    }
}