using Microsoft.EntityFrameworkCore;
using TagStorage.DAL.Entities.Interfaces;

namespace TagStorage.DAL.Entities;

public class FileEntity : EntityWithId
{
    public ICollection<FileTagEntity> FileTags { get; }
    public ICollection<FileLocationEntity> FileLocations { get; }

    public new static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileEntity>()
                    .HasMany(f => f.FileTags)
                    .WithOne(ft => ft.File)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FileEntity>()
                    .HasMany(f => f.FileLocations)
                    .WithOne(fl => fl.File)
                    .OnDelete(DeleteBehavior.Cascade);
    }
}
