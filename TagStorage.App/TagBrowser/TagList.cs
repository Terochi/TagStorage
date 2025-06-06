using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using TagStorage.Library.Entities;
using TagStorage.Library.Repositories;

namespace TagStorage.App.TagBrowser;

public partial class TagList : FillFlowContainer<Tag>
{
    [Resolved]
    private TagRepository tags { get; set; }

    public event Action<Tag> Clicked;

    public void LoadTags(IEnumerable<TagEntity> tags)
    {
        foreach (Tag tag in Children)
        {
            tag.Clicked -= Clicked;
        }

        Clear();

        foreach (TagEntity tag in tags)
        {
            Tag drawable = new Tag(tag);
            drawable.Clicked += Clicked;
            Add(drawable);
        }
    }
}
