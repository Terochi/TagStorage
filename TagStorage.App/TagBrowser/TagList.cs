using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using TagStorage.Library.Entities;
using TagStorage.Library.Repository;

namespace TagStorage.App.TagBrowser;

public partial class TagList : FillFlowContainer<Tag>
{
    [Resolved]
    private TagRepository tags { get; set; }

    public TagList()
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        foreach (TagEntity tag in tags.Get())
        {
            Add(new Tag(tag.Name, Colour4.FromHex(tag.Color)));
        }
    }
}
