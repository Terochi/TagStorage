using Microsoft.EntityFrameworkCore;
using TagStorage.DAL.Entities.Interfaces;

namespace TagStorage.DAL.Entities;

public class DirectoryEntity : EntityWithId
{
    public DirectoryType Type { get; set; }
    public required string Directory { get; set; }

    public new static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DirectoryEntity>()
                    .HasKey(d => d.Id);

        modelBuilder.Entity<DirectoryEntity>()
                    .Property(d => d.Directory)
                    .HasMaxLength(1024);
    }
}

public enum DirectoryType
{
    Included,
    Excluded,
}
