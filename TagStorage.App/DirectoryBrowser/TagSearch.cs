using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using TagStorage.App.TagBrowser;
using TagStorage.Library.Entities;
using TagStorage.Library.Repository;

namespace TagStorage.App.DirectoryBrowser;

public partial class TagSearch : CompositeDrawable
{
    [Resolved]
    private TagRepository tags { get; set; }

    private TagList taglist;
    private TagList selectedlist;
    private BasicTextBox textbox;

    public readonly BindableList<TagEntity> SelectedTags = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        InternalChildren =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.DarkGray,
            },
            new FillFlowContainer
            {
                Direction = FillDirection.Vertical,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Children =
                [
                    textbox = new BasicTextBox
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 35,
                        PlaceholderText = "Search tags...",
                        // Colour = Colour4.FromHex("202020"),
                        // BorderColour = Colour4.FromHex("404040"),
                        CornerRadius = 5,
                        Padding = new MarginPadding(5) { Bottom = 0 },
                        CommitOnFocusLost = false,
                        ReleaseFocusOnCommit = false,
                        Masking = true,
                    },
                    taglist = new TagList
                    {
                        Padding = new MarginPadding(5),
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                    },
                    new SpriteText
                    {
                        Text = "Selected tags:"
                    },
                    selectedlist = new TagList
                    {
                        Padding = new MarginPadding(5),
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                    }
                ]
            }
        ];

        taglist.Clicked += selectTag;
        textbox.OnCommit += onCommit;

        textbox.Current.BindValueChanged(_ =>
        {
            loadTags();
        }, true);
    }

    private void onCommit(TextBox sender, bool newtext)
    {
        Tag first = taglist.FirstOrDefault();

        if (first == null) return;

        selectTag(first);
        sender.Text = string.Empty;
    }

    private void loadTags()
    {
        taglist.LoadTags(tags.Get(textbox.Current.Value).Where(tag => SelectedTags.All(stag => tag.Id != stag.Id)));
    }

    private void selectTag(Tag sender)
    {
        SelectedTags.Add(sender.Entity);
        taglist.Remove(sender, false);
        sender.Clicked -= selectTag;
        sender.Clicked += deselectTag;
        selectedlist.Add(sender);
    }

    private void deselectTag(Tag sender)
    {
        SelectedTags.RemoveAll(stag => stag.Id == sender.Entity.Id);
        selectedlist.Remove(sender, false);
        loadTags();
    }
}
