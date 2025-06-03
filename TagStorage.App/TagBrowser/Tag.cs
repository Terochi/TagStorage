using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using TagStorage.Library.Entities;

namespace TagStorage.App.TagBrowser;

public partial class Tag(TagEntity tag) : CompositeDrawable
{
    public readonly TagEntity Entity = tag;

    [BackgroundDependencyLoader]
    private void load()
    {
        Colour4 color = Colour4.FromHex(Entity.Color);
        Masking = true;
        CornerRadius = 10;
        BorderThickness = 3.5f;
        Colour4 borderColor = color.Lighten(0.2f);
        if (borderColor == color) borderColor = color.Darken(0.2f);
        BorderColour = borderColor;
        AutoSizeAxes = Axes.Both;
        InternalChildren =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = color,
            },
            new SpriteText
            {
                Text = Entity.Name,
                Colour = Colour4.White,
                Margin = new MarginPadding { Left = 5, Right = 5, Top = 2.5f, Bottom = 2.5f },
                Font = FontUsage.Default.With(size: 20),
            }
        ];
    }

    public event Action<Tag> Clicked;

    protected override bool OnClick(ClickEvent e)
    {
        if (Clicked != null)
        {
            Clicked(this);
            return true;
        }

        return base.OnClick(e);
    }
}
