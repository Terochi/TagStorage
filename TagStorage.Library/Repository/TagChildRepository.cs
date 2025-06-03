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

    public virtual TagChildEntity? Get(TagChildEntity tagChild)
    {
        return Connection.ExecuteQuery($"SELECT * FROM {TableName} WHERE child = {tagChild.Child} AND parent = {tagChild.Parent};", MapEntity).FirstOrDefault();
    }

    public override void Delete(TagChildEntity tagChild)
    {
        Connection.ExecuteCommand($"DELETE FROM {TableName} WHERE child = {tagChild.Child} AND parent = {tagChild.Parent};");
    }

    public virtual bool Exists(TagChildEntity tagChild) => Get(tagChild) != null;
}
