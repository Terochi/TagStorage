using Microsoft.EntityFrameworkCore;
using TagStorage.DAL.Entities.Interfaces;

namespace TagStorage.DAL.Entities;

public class FileTagEntity : EntityWithTwoIds
{
    public Guid TagId
    {
        get => Id1;
        set => Id1 = value;
    }

    public TagEntity Tag { get; set; }

    public Guid FileId
    {
        get => Id2;
        set => Id2 = value;
    }

    public FileEntity File { get; set; }

    public new static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileTagEntity>()
                    .Ignore(ft => ft.Id1)
                    .Ignore(ft => ft.Id2);

        modelBuilder.Entity<FileTagEntity>()
                    .HasKey(ft => new { ft.TagId, ft.FileId });

        modelBuilder.Entity<FileTagEntity>()
                    .HasOne(ft => ft.Tag)
                    .WithMany(t => t.FileTags)
                    .HasForeignKey(ft => ft.TagId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FileTagEntity>()
                    .HasOne(ft => ft.File)
                    .WithMany(f => f.FileTags)
                    .HasForeignKey(ft => ft.FileId)
                    .OnDelete(DeleteBehavior.Cascade);
    }
}
