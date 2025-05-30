using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;

namespace TagStorage.App.TagBrowser;

public partial class Tag(string name, Colour4 color) : CompositeDrawable
{
    [BackgroundDependencyLoader]
    private void load()
    {
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
                Text = name,
                Colour = Colour4.White,
                Margin = new MarginPadding { Left = 5, Right = 5, Top = 2.5f, Bottom = 2.5f },
                Font = FontUsage.Default.With(size: 20),
            }
        ];
    }
}
