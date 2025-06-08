using Microsoft.EntityFrameworkCore;

namespace TagStorage.DAL.Entities.Interfaces;

public abstract class EntityWithId : IEntity
{
    public Guid Id { get; set; }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        throw new NotSupportedException();
    }
}
