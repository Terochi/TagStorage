using Microsoft.EntityFrameworkCore;
using TagStorage.DAL.Entities.Interfaces;

namespace TagStorage.DAL.Entities;

public class TagEntity : EntityWithId
{
    public required string Name { get; set; }
    public string? Color { get; set; }

    public ICollection<TagChildEntity> Parents { get; }
    public ICollection<TagChildEntity> Children { get; }
    public ICollection<FileTagEntity> FileTags { get; }

    public new static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TagEntity>()
                    .HasMany(t => t.Parents)
                    .WithOne(tc => tc.Child)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TagEntity>()
                    .HasMany(t => t.Children)
                    .WithOne(tc => tc.Parent)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TagEntity>()
                    .HasMany(t => t.FileTags)
                    .WithOne(ft => ft.Tag)
                    .OnDelete(DeleteBehavior.Cascade);
    }
}
