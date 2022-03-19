using DUT.Domain.Models;
using DUT.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DUT.Infrastructure.Data.Configurations
{
    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Holidays).HasConversion(
                v => v.ToJson(),
                v => v.FromJson<IEnumerable<Holiday>>());

            builder.Property(x => x.LessonTimes).HasConversion(
                v => v.ToJson(),
                v => v.FromJson<IEnumerable<LessonTime>>());
        }
    }
}