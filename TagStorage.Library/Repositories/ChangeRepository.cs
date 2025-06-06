using System.Data;
using TagStorage.Library.Entities;

namespace TagStorage.Library.Repositories;

public partial class ChangeRepository : BaseRepository<ChangeEntity>
{
    protected override string TableName => "changes";

    protected override ChangeEntity MapEntity(IDataReader reader) =>
        new ChangeEntity
        {
            Id = reader.GetInt32(0),
            Location = reader.GetInt32(1),
            Date = reader.GetDateTime(2),
            Size = reader.GetInt64(3),
            Hash = reader.IsDBNull(4) ? null : reader.GetString(4)
        };

    public IEnumerable<ChangeEntity> FindDuplicates(ChangeEntity entity)
    {
        return Connection.ExecuteQuery($"SELECT * FROM {TableName} WHERE size = {entity.Size} AND hash = '{entity.Hash}';", MapEntity);
    }
}
