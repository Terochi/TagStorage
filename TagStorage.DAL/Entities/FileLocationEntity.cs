using Microsoft.EntityFrameworkCore;
using TagStorage.DAL.Entities.Interfaces;

namespace TagStorage.DAL.Entities;

public class FileLocationEntity : EntityWithId
{
    public FileLocationType Type { get; set; }
    public required string Path { get; set; }
    public required string Machine { get; set; }

    public Guid FileId { get; set; }
    public FileEntity File { get; }

    public ICollection<ChangeEntity> Changes { get; }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileLocationEntity>()
                    .HasOne(f => f.File)
                    .WithMany()
                    .HasForeignKey(f => f.FileId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FileLocationEntity>()
                    .HasMany(c => c.Changes)
                    .WithOne(c => c.FileLocation)
                    .OnDelete(DeleteBehavior.Cascade);
    }
}

public enum FileLocationType
{
    File,
    Directory,
}
