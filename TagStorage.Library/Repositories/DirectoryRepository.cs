using System.Data;
using TagStorage.Library.Entities;

namespace TagStorage.Library.Repositories;

public partial class DirectoryRepository : BaseRepository<DirectoryEntity>
{
    protected override string TableName => "directories";

    protected override DirectoryEntity MapEntity(IDataReader reader) =>
        new DirectoryEntity
        {
            Id = reader.GetInt32(0),
            Type = reader.GetString(1) switch
            {
                "I" => DirectoryType.I,
                "E" => DirectoryType.E,
                _ => throw new ArgumentOutOfRangeException()
            },
            Directory = reader.GetString(2),
        };
}
