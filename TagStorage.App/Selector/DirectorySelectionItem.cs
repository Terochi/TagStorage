using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK;

namespace TagStorage.App.Selector;

public partial class DirectorySelectionItem : SelectionItem<string>
{
    [Resolved]
    private GameHost host { get; set; } = null!;

    private Box selection;

    public DirectorySelectionItem(string item)
        : base(item)
    {
        StateChanged += updateState;
    }

    protected override Drawable CreateSelection() => selection = new Box
    {
        RelativeSizeAxes = Axes.Both,
        Colour = Colour4.White,
    };

    protected override Drawable CreateContent() => new FillFlowContainer
    {
        Direction = FillDirection.Horizontal,
        AutoSizeAxes = Axes.Both,
        Margin = new MarginPadding(5),
        Spacing = new Vector2(5),
        Children =
        [
            new SpriteIcon
            {
                Icon = FontAwesome.Solid.Folder,
                Size = new Vector2(20)
            },
            new SpriteText
            {
                Width = 200,
                Font = FrameworkFont.Condensed,
                Text = Item,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
            }
        ]
    };

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
        selection.Alpha = getAlpha();
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
