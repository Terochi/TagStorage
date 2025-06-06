using System.Data;
using TagStorage.Library.Entities;

namespace TagStorage.Library.Repositories;

public partial class TaggingRuleRepository : BaseRepository<TaggingRuleEntity>
{
    protected override string TableName => "tagging_rules";

    protected override TaggingRuleEntity MapEntity(IDataReader reader) =>
        new TaggingRuleEntity
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
        };
}
