using Microsoft.EntityFrameworkCore;
using TagStorage.DAL.Entities.Interfaces;
using TagStorage.DAL.Mappers;
using TagStorage.DAL.Repositories;

namespace TagStorage.DAL.UnitOfWork;

public sealed class UnitOfWork(DbContext dbContext)
{
    private readonly DbContext dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public Repository<TEntity> GetRepository<TEntity, TEntityMapper>()
        where TEntity : class, IEntity
        where TEntityMapper : IEntityMapper<TEntity>, new() =>
        new Repository<TEntity>(dbContext, new TEntityMapper());

    public async Task CommitAsync() => await dbContext.SaveChangesAsync().ConfigureAwait(false);

    public async ValueTask DisposeAsync() => await dbContext.DisposeAsync().ConfigureAwait(false);
}
