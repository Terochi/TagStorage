using Microsoft.EntityFrameworkCore;

namespace TagStorage.DAL.Entities.Interfaces;

public interface IEntity
{
    public abstract static void OnModelCreating(ModelBuilder modelBuilder);
}
