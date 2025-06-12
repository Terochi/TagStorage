using System.Data;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using TagStorage.Library.Entities;

namespace TagStorage.Library.Repositories;

public abstract partial class BaseRepository<TEntity> : IDependencyInjectionCandidate
    where TEntity : class, IEntity
{
    protected abstract string TableName { get; }

    [Resolved]
    protected DatabaseConnection Connection { get; private set; }

    protected abstract TEntity MapEntity(IDataReader reader);

    public virtual bool Exists(int id) => Get(id) != null;

    [CanBeNull]
    public virtual TEntity Get(int id)
    {
        return Connection.ExecuteQuery($"SELECT * FROM {TableName} WHERE id = {id};", MapEntity).FirstOrDefault();
    }

    public virtual IEnumerable<TEntity> Get()
    {
        return Connection.ExecuteQuery($"SELECT * FROM {TableName};", MapEntity);
    }

    public virtual TEntity Insert(TEntity entity)
    {
        string names = string.Join(", ", entity.GetFieldNames());
        string values = string.Join(", ", entity.GetFieldValues());

        return Connection.ExecuteQuery($"INSERT INTO {TableName} ({names}) VALUES ({values}) RETURNING *;", MapEntity)
                         .First();
    }

    public virtual TEntity Update(TEntity entity)
    {
        string setClause = string.Join(", ", entity.GetFieldNames().Zip(entity.GetFieldValues(), (n, v) => $"{n} = {v}"));

        return Connection.ExecuteQuery($"UPDATE {TableName} SET {setClause} WHERE id = {entity.Id} RETURNING *;", MapEntity)
                         .First();
    }

    public virtual void Delete(TEntity entity)
    {
        Connection.ExecuteCommand($"DELETE FROM {TableName} WHERE id = {entity.Id};");
    }
}
