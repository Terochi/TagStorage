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
            Location = reader.GetInt32(1)
        };
}
