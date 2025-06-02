using System.Data;
using TagStorage.Library.Entities;

namespace TagStorage.Library.Repository;

public class DirectoryRepository(DatabaseConnection connection) : BaseRepository<DirectoryEntity>(connection)
{
    protected override string TableName => "directories";

    protected override DirectoryEntity MapEntity(IDataReader reader) =>
        new DirectoryEntity
        {
            Id = reader.GetInt32(0),
            Type = reader.GetString(1) switch
            {
                "I" => DirectoryType.Included,
                "E" => DirectoryType.Excluded,
                _ => throw new ArgumentOutOfRangeException()
            },
            Directory = reader.GetString(2),
        };
}
