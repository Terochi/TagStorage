using osu.Framework.Allocation;
using TagStorage.Library.Entities;
using TagStorage.Library.Repositories;

namespace TagStorage.Library.Facades;

public partial class AutoTagFacade : IFacadeBase
{
    [Resolved]
    private AutomaticTagRepository autoTags { get; set; }

    [Resolved]
    private TaggingRuleRepository tagRules { get; set; }

    public IEnumerable<AutomaticTagEntity> Get(string directory)
    {
        return autoTags.Get(directory);
    }

    public IEnumerable<TaggingRuleEntity> GetTagRules()
    {
        return tagRules.Get();
    }

    public void Insert(AutomaticTagEntity tag)
    {
        autoTags.Insert(tag);
    }
}
