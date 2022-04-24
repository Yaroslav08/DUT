using URLS.Domain.Models;
using URLS.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace URLS.Infrastructure.Data.Configurations
{
    public class QuizResultConfiguration : IEntityTypeConfiguration<QuizResult>
    {
        public void Configure(EntityTypeBuilder<QuizResult> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Statistics).HasConversion(
                v => v.ToJson(),
                v => v.FromJson<QuizResultStatistics>());

            builder.Property(s => s.Answers).HasConversion(
                v => v.ToJson(),
                v => v.FromJson<List<QuizAnswer>>());
        }
    }
}