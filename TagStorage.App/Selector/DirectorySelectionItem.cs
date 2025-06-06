using System.IO;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK;
using TagStorage.App.TagBrowser;
using TagStorage.Library.Entities;
using TagStorage.Library.Repositories;

namespace TagStorage.App.Selector;

public partial class DirectorySelectionItem : SelectionItem<string>
{
    public string FullName => Path.Join(CurrentDirectory.Value, Item);
    public Bindable<string> CurrentDirectory { get; set; } = new Bindable<string>();

    [Resolved]
    private GameHost host { get; set; } = null!;

    [Resolved]
    private TagRepository tags { get; set; }

    [Resolved]
    private ChangeRepository changes { get; set; }

    [Resolved]
    private FileRepository files { get; set; }

    [Resolved]
    private FileLocationRepository fileLocations { get; set; }

    [Resolved]
    private FileTagRepository fileTags { get; set; }

    private Box selection;

    private TagList tagList;

    public DirectorySelectionItem(string item)
        : base(item)
    {
        StateChanged += updateState;
    }

    protected virtual Drawable CreateIcon(GameHost host) =>
        new SpriteIcon
        {
            Icon = FontAwesome.Solid.Folder,
            Size = new Vector2(20)
        };

    protected override Drawable CreateSelection() => selection = new Box
    {
        RelativeSizeAxes = Axes.Both,
        Colour = Colour4.White,
    };

    protected override Drawable CreateContent(GameHost host)
    {
        var content = new FillFlowContainer
        {
            Direction = FillDirection.Horizontal,
            AutoSizeAxes = Axes.Both,
            Margin = new MarginPadding(5),
            Spacing = new Vector2(5),
            Children =
            [
                CreateIcon(host),
                new SpriteText
                {
                    Width = 200,
                    Font = FrameworkFont.Condensed,
                    Text = Item,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                },
                tagList = new TagList
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                }
            ]
        };
        tagList.Clicked += tag =>
        {
            foreach (int fileId in fileLocations.GetByPath(Path.Join(CurrentDirectory.Value, Item)).Select(
                         loc => files.Get(loc.File)!.Id))
            {
                fileTags.Delete(new FileTagEntity { File = fileId, Tag = tag.Entity.Id });
            }

            LoadTags();
        };
        Schedule(LoadTags);
        return content;
    }

    public void LoadTags()
    {
        var setTags = fileLocations.GetByPath(Path.Join(CurrentDirectory.Value, Item)).Select(
                                       loc => files.Get(loc.File)!.Id)
                                   .SelectMany(fileTags.GetByFile)
                                   .Select(fileTag => tags.Get(fileTag.Tag));
        tagList.LoadTags(setTags);
    }

    protected override void LoadComplete()
    {
        host.IsActive.BindValueChanged(_ => updateSelection(), true);

        base.LoadComplete();
    }

    protected override bool OnHover(HoverEvent e)
    {
        updateSelection();
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        updateSelection();
        base.OnHoverLost(e);
    }

    private void updateState(SelectionState state)
    {
        updateSelection();
    }

    private void updateSelection()
    {
        Schedule(() => selection.Alpha = getAlpha());
    }

    private float getAlpha()
    {
        const float hover_alpha = 0.2f;
        const float select_active_alpha = 0.4f;
        const float select_inactive_alpha = 0.1f;

        bool selected = IsSelected;
        bool hovered = IsHovered;
        bool opened = host.IsActive.Value;

        if (opened)
        {
            if (selected)
            {
                return select_active_alpha;
            }

            if (hovered)
            {
                return hover_alpha;
            }

            return 0f;
        }

        if (hovered)
        {
            return hover_alpha;
        }

        if (selected)
        {
            return select_inactive_alpha;
        }

        return 0f;
    }
}
