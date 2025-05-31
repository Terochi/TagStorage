using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;

namespace TagStorage.App.Selector;

public partial class TestSelectItem : CompositeDrawable, ISelectable<string>
{
    public string GetUnderlyingData()
    {
        throw new System.NotImplementedException();
    }

    public bool IsSelected { get; set; }

    public void OnSelected()
    {
        select.FadeIn();

        IsSelected = true;
    }

    public void OnDeselected()
    {
        select.FadeOut();

        IsSelected = false;
    }

    private Box select;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        InternalChildren =
        [
            select = new Box()
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FrameworkColour.YellowGreen.Opacity(0.5f),
                Alpha = 0f
            },
            new SpriteText()
            {
                Text = "Test Select Item",
            }
        ];
    }
}
