using System.Data;
using TagStorage.Library.Entities;

namespace TagStorage.Library.Repository;

public class FileLocationRepository(DatabaseConnection connection) : BaseRepository<FileLocationEntity>(connection)
{
    protected override string TableName => "file_locations";

    protected override FileLocationEntity MapEntity(IDataReader reader) =>
        new FileLocationEntity
        {
            Id = reader.GetInt32(0),
            Type = reader.GetString(1) switch
            {
                "F" => FileLocationType.File,
                "D" => FileLocationType.Directory,
                _ => throw new ArgumentOutOfRangeException()
            },
            Path = reader.GetString(2),
            Machine = reader.GetString(3),
        };
}
