using Microsoft.EntityFrameworkCore;
using TagStorage.DAL.Entities.Interfaces;

namespace TagStorage.DAL.Entities;

public class TaggingRuleEntity : EntityWithId
{
    public required string Name { get; set; }

    public ICollection<AutoTagEntity> AutomaticTags { get; }

    public new static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaggingRuleEntity>()
                    .HasMany(t => t.AutomaticTags)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
    }
}
