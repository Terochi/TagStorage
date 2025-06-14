﻿using Microsoft.EntityFrameworkCore;
using TagStorage.DAL.Entities.Interfaces;

namespace TagStorage.DAL.Entities;

public class AutoTagEntity : EntityWithId
{
    public required string Directory { get; set; }

    public Guid TagRuleId { get; set; }
    public TaggingRuleEntity TagRule { get; set; }

    public new static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AutoTagEntity>()
                    .HasKey(e => e.Id);

        modelBuilder.Entity<AutoTagEntity>()
                    .HasOne(e => e.TagRule)
                    .WithMany()
                    .HasForeignKey(e => e.TagRuleId)
                    .OnDelete(DeleteBehavior.Cascade);
    }
}
