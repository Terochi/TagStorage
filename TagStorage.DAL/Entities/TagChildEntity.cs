using Microsoft.EntityFrameworkCore;
using TagStorage.DAL.Entities.Interfaces;

namespace TagStorage.DAL.Entities;

public class TagChildEntity : EntityWithTwoIds
{
    public Guid ChildId
    {
        get => Id1;
        set => Id1 = value;
    }

    public TagEntity Child { get; }

    public Guid ParentId
    {
        get => Id2;
        set => Id2 = value;
    }

    public TagEntity Parent { get; }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TagChildEntity>()
                    .Ignore(tc => tc.Id1)
                    .Ignore(tc => tc.Id2);

        modelBuilder.Entity<TagChildEntity>()
                    .HasKey(tc => new { tc.ChildId, tc.ParentId });

        modelBuilder.Entity<TagChildEntity>()
                    .HasOne(tc => tc.Child)
                    .WithMany(t => t.Parents)
                    .HasForeignKey(tc => tc.ChildId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TagChildEntity>()
                    .HasOne(tc => tc.Parent)
                    .WithMany(t => t.Children)
                    .HasForeignKey(tc => tc.ParentId)
                    .OnDelete(DeleteBehavior.Cascade);
    }
}
