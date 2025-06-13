using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osuTK;
using TagStorage.App.TagBrowser;
using TagStorage.Library.Facades;

namespace TagStorage.App.DirectoryBrowser;

public partial class TagSearchBox : CompositeDrawable
{
    [Resolved]
    private TagFacade tags { get; set; }

    private TagList taglist;
    private BasicTextBox textbox;

    public event TextBox.OnCommitHandler OnCommit
    {
        add => textbox.OnCommit += value;
        remove => textbox.OnCommit -= value;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        InternalChildren =
        [
            new Box()
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.DarkGray,
            },
            new FillFlowContainer
            {
                Direction = FillDirection.Vertical,
                AutoSizeAxes = Axes.Both,
                Children =
                [
                    textbox = new BasicTextBox
                    {
                        Size = new Vector2(300, 30),
                        PlaceholderText = "Search tags...",
                        // Colour = Colour4.FromHex("202020"),
                        // BorderColour = Colour4.FromHex("404040"),
                        CornerRadius = 5,
                        Padding = new MarginPadding(5) { Bottom = 0 },
                        CommitOnFocusLost = false,
                        Masking = true,
                    },
                    taglist = new TagList
                    {
                        Padding = new MarginPadding(5),
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                    }
                ]
            }
        ];

        taglist.Clicked += clicked;

        tags.Tags.BindCollectionChanged((_, _) =>
        {
            taglist.LoadTags(tags.Get(textbox.Text));
        });

        textbox.Current.BindValueChanged(text =>
        {
            taglist.LoadTags(tags.Get(text.NewValue));
        }, true);
    }

    private void clicked(Tag sender)
    {
        tags.Delete(sender.Entity);
    }

    public override void Show()
    {
        textbox.Text = string.Empty;
        Scheduler.Add(() => GetContainingFocusManager()!.ChangeFocus(textbox));

        base.Show();
    }
}
