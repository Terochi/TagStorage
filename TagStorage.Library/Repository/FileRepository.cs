using System.Data;
using TagStorage.Library.Entities;

namespace TagStorage.Library.Repository;

public class FileRepository(DatabaseConnection connection) : BaseRepository<FileEntity>(connection)
{
    protected override string TableName => "files";

    protected override FileEntity MapEntity(IDataReader reader) =>
        new FileEntity
        {
            Id = reader.GetInt32(0),
        };

    public override FileEntity Insert(FileEntity entity)
    {
        return Connection.ExecuteQuery($"INSERT INTO {TableName} DEFAULT VALUES RETURNING *;", MapEntity)
                         .First();
    }
}
