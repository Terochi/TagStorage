using System.Data;
using TagStorage.Library.Entities;

namespace TagStorage.Library.Repository;

public class AutomaticTagRepository(DatabaseConnection connection) : BaseRepository<AutomaticTagEntity>(connection)
{
    protected override string TableName => "automatic_tags";

    protected override AutomaticTagEntity MapEntity(IDataReader reader) =>
        new AutomaticTagEntity
        {
            Id = reader.GetInt32(0),
            Rule = reader.GetInt32(1),
            Directory = reader.GetString(2)
        };

    public IEnumerable<AutomaticTagEntity> Get(string directory)
    {
        return Connection.ExecuteQuery($"SELECT * FROM {TableName} WHERE directory = '{directory}';", MapEntity);
    }
}
