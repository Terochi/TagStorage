using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TagStorage.DAL.Entities;
using TagStorage.DAL.Entities.Interfaces;

namespace TagStorage.DAL;

public class TagStorageDbContext(DbContextOptions contextOptions) : DbContext(contextOptions)
{
    public DbSet<AutoTagEntity> AutoTagEntities => Set<AutoTagEntity>();
    public DbSet<TaggingRuleEntity> TaggingRuleEntities => Set<TaggingRuleEntity>();
    public DbSet<TagEntity> TagEntities => Set<TagEntity>();
    public DbSet<FileTagEntity> FileTagEntities => Set<FileTagEntity>();
    public DbSet<ChangeEntity> ChangeEntities => Set<ChangeEntity>();
    public DbSet<DirectoryEntity> DirectoryEntities => Set<DirectoryEntity>();
    public DbSet<FileEntity> FileEntities => Set<FileEntity>();
    public DbSet<FileLocationEntity> FileLocationEntities => Set<FileLocationEntity>();
    public DbSet<TagChildEntity> TagChildEntities => Set<TagChildEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var methods =
            typeof(TagStorageDbContext)
                .Assembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IEntity)) && !t.IsInterface & !t.IsAbstract)
                .Select(t => t.GetMethod(nameof(IEntity.OnModelCreating), BindingFlags.Static | BindingFlags.Public)!);

        object?[] parameters = [modelBuilder];

        foreach (MethodInfo onModelCreating in methods)
        {
            onModelCreating.Invoke(null, parameters);
        }
    }
}
