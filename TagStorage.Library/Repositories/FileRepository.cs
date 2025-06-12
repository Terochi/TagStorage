using System.Collections.Specialized;
using System.Data;
using TagStorage.Library.Entities;

namespace TagStorage.Library.Repositories;

public partial class FileRepository : BaseRepository<FileEntity>
{
    protected override string TableName => "files";

    protected override FileEntity MapEntity(IDataReader reader) =>
        new FileEntity
        {
            Id = reader.GetInt32(0),
        };

    public override FileEntity Insert(FileEntity entity)
    {
        var fileEntity = Connection.ExecuteQuery($"INSERT INTO {TableName} DEFAULT VALUES RETURNING *;", MapEntity)
                                   .First();
        NotifyChange(NotifyCollectionChangedAction.Add, fileEntity);
        return fileEntity;
    }
}
