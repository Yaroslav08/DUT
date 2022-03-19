using DUT.Domain.Models;
using DUT.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DUT.Infrastructure.Data.Configurations
{
    public class TimetableConfiguration : IEntityTypeConfiguration<Timetable>
    {
        public void Configure(EntityTypeBuilder<Timetable> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Holiday).HasConversion(
                v => v.ToJson(),
                v => v.FromJson<Holiday>());

            builder.Property(x => x.Time).HasConversion(
                v => v.ToJson(),
                v => v.FromJson<LessonTime>());
        }
    }
}