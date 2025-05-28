using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osuTK;

namespace TagStorage.App.Directory;

public partial class DirectoryItem : Container
{
    private const float hover_alpha = 0.2f;
    private const float select_active_alpha = 0.4f;
    private const float select_inactive_alpha = 0.1f;

    private float getAlpha()
    {
        if (host.IsActive.Value)
        {
            if (HasFocus)
            {
                return select_active_alpha;
            }

            if (IsHovered)
            {
                return hover_alpha;
            }

            return 0f;
        }

        if (IsHovered)
        {
            return hover_alpha;
        }

        if (HasFocus)
        {
            return select_inactive_alpha;
        }

        return 0f;
    }

    private float selectAlpha => host.IsActive.Value ? select_active_alpha : select_inactive_alpha;

    [Resolved]
    private GameHost host { get; set; } = null!;

    private Box hover;
    private SpriteIcon icon;
    private SpriteText text;

    public override bool AcceptsFocus => true;

    public LocalisableString Text
    {
        get => text.Text;
        set => text.Text = value;
    }

    public IconUsage Icon
    {
        get => icon.Icon;
        set => icon.Icon = value;
    }

    public new ColourInfo Colour
    {
        get => icon.Colour;
        set => icon.Colour = value;
    }

    public DirectoryItem()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.TopLeft;
        Origin = Anchor.TopLeft;

        icon = new SpriteIcon
        {
            Size = new Vector2(20),
            Anchor = Anchor.TopLeft,
            Origin = Anchor.TopLeft,
            Margin = new MarginPadding(1)
        };
        text = new SpriteText
        {
            X = 25,
            Font = FontUsage.Default.With(size: 15),
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
        };
    }

    private Action action;

    public Action Action
    {
        get => action;
        set
        {
            action = value;
            Enabled.Value = action != null;
        }
    }

    public readonly BindableBool Enabled = new BindableBool();

    protected override bool OnClick(ClickEvent e)
    {
        return true;
    }

    protected override bool OnDoubleClick(DoubleClickEvent e)
    {
        if (Enabled.Value)
            Action?.Invoke();
        return true;
    }

    protected override bool Handle(UIEvent e)
    {
        switch (e)
        {
            case FocusEvent _:
            case FocusLostEvent _:
            case HoverEvent _:
            case HoverLostEvent _:
                hover.Alpha = getAlpha();
                break;
        }

        return base.Handle(e);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            hover = new Box
            {
                Colour = Colour4.White,
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
            },
            new Container
            {
                AutoSizeAxes = Axes.Both,
                Padding = new MarginPadding() { Left = 5, Right = 5 },
                Children =
                [
                    icon,
                    text
                ]
            }
        ];

        host.IsActive.BindValueChanged(_ =>
        {
            if (HasFocus) hover.Alpha = selectAlpha;
        });
    }
}
