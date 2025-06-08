using Microsoft.EntityFrameworkCore;
using TagStorage.DAL.Entities.Interfaces;

namespace TagStorage.DAL.Entities;

public class ChangeEntity : EntityWithId
{
    public int Location { get; set; }
    public DateTime Date { get; set; }
    public long Size { get; set; }
    public string? Hash { get; set; }

    public Guid FileLocationId { get; set; }
    public FileLocationEntity FileLocation { get; }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChangeEntity>()
                    .HasKey(c => c.Id);

        modelBuilder.Entity<ChangeEntity>()
                    .Property(c => c.Hash)
                    .HasMaxLength(64);

        modelBuilder.Entity<ChangeEntity>()
                    .HasOne(c => c.FileLocation)
                    .WithMany()
                    .HasForeignKey(c => c.FileLocationId)
                    .OnDelete(DeleteBehavior.Cascade);
    }
}
