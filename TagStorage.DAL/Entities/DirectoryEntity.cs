using Microsoft.EntityFrameworkCore;
using TagStorage.DAL.Entities.Interfaces;

namespace TagStorage.DAL.Entities;

public class DirectoryEntity : EntityWithId
{
    public DirectoryType Type { get; set; }
    public required string Directory { get; set; }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DirectoryEntity>()
                    .HasKey(d => d.Id);
    }
}

public enum DirectoryType
{
    Included,
    Excluded,
}
