﻿using DUT.Domain.Models;
using DUT.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DUT.Infrastructure.Data.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Journal).HasConversion(
                v => v.ToJson(),
                v => v.FromJson<Journal>());
        }
    }
}