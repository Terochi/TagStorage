using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TagStorage.DAL.Entities.Interfaces;
using TagStorage.DAL.Mappers;

namespace TagStorage.DAL.Repositories;

public class Repository<TEntity>(
    DbContext dbContext,
    IEntityMapper<TEntity> entityMapper)
    where TEntity : class, IEntity
{
    private readonly DbSet<TEntity> dbSet = dbContext.Set<TEntity>();

    public IQueryable<TEntity> Get() => dbSet;

    public async ValueTask<bool> ExistsAsync(TEntity entity)
        => await dbSet.AnyAsync(equalIds(entity)).ConfigureAwait(false);

    public TEntity Insert(TEntity entity)
        => dbSet.Add(entity).Entity;

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        TEntity existingEntity = await dbSet.SingleAsync(equalIds(entity)).ConfigureAwait(false);
        entityMapper.MapToExistingEntity(existingEntity, entity);
        return existingEntity;
    }

    public async Task DeleteAsync(TEntity entity)
        => dbSet.Remove(await dbSet.SingleAsync(equalIds(entity)).ConfigureAwait(false));

    private Expression<Func<TEntity, bool>> equalIds(TEntity entity)
    {
        if (entity is EntityWithId)
        {
            return e => (e as EntityWithId)!.Id == (entity as EntityWithId)!.Id;
        }

        if (entity is EntityWithTwoIds)
        {
            return e => (e as EntityWithTwoIds)!.Id1 == (entity as EntityWithTwoIds)!.Id1 &&
                        (e as EntityWithTwoIds)!.Id2 == (entity as EntityWithTwoIds)!.Id2;
        }

        throw new InvalidOperationException("Entity type does not have an Id or Ids for comparison.");
    }
}
