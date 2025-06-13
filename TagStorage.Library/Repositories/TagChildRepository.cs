using System.Collections.Specialized;
using System.Data;
using JetBrains.Annotations;
using TagStorage.Library.Entities;

namespace TagStorage.Library.Repositories;

public partial class TagChildRepository : BaseRepository<TagChildEntity>
{
    protected override string TableName => "tag_children";

    protected override TagChildEntity MapEntity(IDataReader reader) =>
        new TagChildEntity
        {
            Child = reader.GetInt32(0),
            Parent = reader.GetInt32(1)
        };

    [CanBeNull]
    public TagChildEntity Get(TagChildEntity tagChild)
    {
        return Connection.ExecuteQuery($"SELECT * FROM {TableName} WHERE child = {tagChild.Child} AND parent = {tagChild.Parent};", MapEntity).FirstOrDefault();
    }

    [CanBeNull]
    public IEnumerable<TagChildEntity> GetParents(int childId)
    {
        return Connection.ExecuteQuery($"SELECT * FROM {TableName} WHERE child = {childId};", MapEntity);
    }

    [CanBeNull]
    public IEnumerable<TagChildEntity> GetChildren(int parentId)
    {
        return Connection.ExecuteQuery($"SELECT * FROM {TableName} WHERE parent = {parentId};", MapEntity);
    }

    public override void Delete(TagChildEntity tagChild)
    {
        Connection.ExecuteCommand($"DELETE FROM {TableName} WHERE child = {tagChild.Child} AND parent = {tagChild.Parent};");
        NotifyChange(NotifyCollectionChangedAction.Remove, tagChild);
    }

    public virtual bool Exists(TagChildEntity tagChild) => Get(tagChild) != null;
}
