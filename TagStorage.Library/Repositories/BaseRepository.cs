using System.Collections.Specialized;
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

    public delegate void RepositoryChangeEventHandler(BaseRepository<TEntity> query, NotifyCollectionChangedEventArgs e);

    public event RepositoryChangeEventHandler RepositoryChanged;

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

        var tEntity = Connection.ExecuteQuery($"INSERT INTO {TableName} ({names}) VALUES ({values}) RETURNING *;", MapEntity)
                                .First();
        NotifyChange(NotifyCollectionChangedAction.Add, tEntity);
        return tEntity;
    }

    public virtual TEntity Update(TEntity entity)
    {
        string setClause = string.Join(", ", entity.GetFieldNames().Zip(entity.GetFieldValues(), (n, v) => $"{n} = {v}"));

        var tEntity = Connection.ExecuteQuery($"UPDATE {TableName} SET {setClause} WHERE id = {entity.Id} RETURNING *;", MapEntity)
                                .First();
        NotifyChange(NotifyCollectionChangedAction.Replace, tEntity);
        return tEntity;
    }

    public virtual void Delete(TEntity entity)
    {
        Connection.ExecuteCommand($"DELETE FROM {TableName} WHERE id = {entity.Id};");
        NotifyChange(NotifyCollectionChangedAction.Remove, entity);
    }

    protected void NotifyChange(NotifyCollectionChangedAction action, TEntity entity) =>
        RepositoryChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, entity));
}
