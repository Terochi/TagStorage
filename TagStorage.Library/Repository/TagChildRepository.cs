using System.Data;
using TagStorage.Library.Entities;

namespace TagStorage.Library.Repository;

public class TagChildRepository(DatabaseConnection connection) : BaseRepository<TagChildEntity>(connection)
{
    protected override string TableName => "tag_children";

    protected override TagChildEntity MapEntity(IDataReader reader) =>
        new TagChildEntity
        {
            Child = reader.GetInt32(0),
            Parent = reader.GetInt32(1)
        };
}
