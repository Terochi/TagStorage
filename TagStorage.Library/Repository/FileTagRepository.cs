using System.Data;
using TagStorage.Library.Entities;

namespace TagStorage.Library.Repository;

public class FileTagRepository(DatabaseConnection connection) : BaseRepository<FileTagEntity>(connection)
{
    protected override string TableName => "file_tags";

    protected override FileTagEntity MapEntity(IDataReader reader) =>
        new FileTagEntity
        {
            Tag = reader.GetInt32(0),
            File = reader.GetInt32(1)
        };
}
