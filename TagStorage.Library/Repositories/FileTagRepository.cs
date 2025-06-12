using System.Collections.Specialized;
using System.Data;
using JetBrains.Annotations;
using TagStorage.Library.Entities;

namespace TagStorage.Library.Repositories;

public partial class FileTagRepository : BaseRepository<FileTagEntity>
{
    protected override string TableName => "file_tags";

    protected override FileTagEntity MapEntity(IDataReader reader) =>
        new FileTagEntity
        {
            Tag = reader.GetInt32(0),
            File = reader.GetInt32(1)
        };

    public virtual IEnumerable<FileTagEntity> GetByFile(int fileId)
    {
        return Connection.ExecuteQuery($"SELECT * FROM {TableName} WHERE file = {fileId};", MapEntity);
    }

    public virtual IEnumerable<FileTagEntity> GetByTag(int tagId)
    {
        return Connection.ExecuteQuery($"SELECT * FROM {TableName} WHERE tag = {tagId};", MapEntity);
    }

    [CanBeNull]
    public virtual FileTagEntity Get(FileTagEntity fileTag)
    {
        return Connection.ExecuteQuery($"SELECT * FROM {TableName} WHERE tag = {fileTag.Tag} AND file = {fileTag.File};", MapEntity).FirstOrDefault();
    }

    public override void Delete(FileTagEntity fileTag)
    {
        Connection.ExecuteCommand($"DELETE FROM {TableName} WHERE tag = {fileTag.Tag} AND file = {fileTag.File};");
        NotifyChange(NotifyCollectionChangedAction.Remove, fileTag);
    }

    public virtual bool Exists(FileTagEntity fileTag) => Get(fileTag) != null;
}
