using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using TagStorage.App.Selector;

namespace TagStorage.App.DirectoryBrowser;

public partial class DirectoryContainer : CompositeDrawable
{
    public DirectoryContainer()
    {
        Masking = true;
        CornerRadius = 10;
    }

    public DirectorySelectionContainer DirectorySelectionContainer { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.FromHex("202020"),
            },
            new BasicScrollContainer
            {
                ClampExtension = 0,
                DistanceDecayScroll = 1f,
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(20),
                Child = DirectorySelectionContainer = new DirectorySelectionContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                }
            }
        ];
    }
}
