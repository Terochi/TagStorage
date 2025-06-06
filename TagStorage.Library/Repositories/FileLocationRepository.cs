using System.Data;
using TagStorage.Library.Entities;

namespace TagStorage.Library.Repositories;

public partial class FileLocationRepository : BaseRepository<FileLocationEntity>
{
    protected override string TableName => "file_locations";

    protected override FileLocationEntity MapEntity(IDataReader reader) =>
        new FileLocationEntity
        {
            Id = reader.GetInt32(0),
            File = reader.GetInt32(1),
            Type = reader.GetString(2) switch
            {
                "F" => FileLocationType.F,
                "D" => FileLocationType.D,
                _ => throw new ArgumentOutOfRangeException()
            },
            Path = reader.GetString(3),
            Machine = reader.GetString(4),
        };

    public IEnumerable<FileLocationEntity> GetByPath(string path)
    {
        return Connection.ExecuteQuery($"SELECT * FROM {TableName} WHERE path = '{path}';", MapEntity);
    }
}
