using Microsoft.EntityFrameworkCore;

namespace TagStorage.DAL.Factories;

public class DbContextSqliteFactory : IDbContextFactory<TagStorageDbContext>
{
    private readonly DbContextOptionsBuilder<TagStorageDbContext> contextOptionsBuilder = new DbContextOptionsBuilder<TagStorageDbContext>();

    public DbContextSqliteFactory(string databaseName)
    {
        contextOptionsBuilder.UseSqlite($"Data Source={databaseName};Cache=Shared");
    }

    public TagStorageDbContext CreateDbContext() => new TagStorageDbContext(contextOptionsBuilder.Options);
}
