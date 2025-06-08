using Microsoft.EntityFrameworkCore;

namespace TagStorage.DAL.Entities.Interfaces;

public abstract class EntityWithTwoIds : IEntity
{
    public Guid Id1 { get; set; }
    public Guid Id2 { get; set; }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        throw new NotSupportedException();
    }
}
