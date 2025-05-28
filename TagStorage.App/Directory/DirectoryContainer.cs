using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Textures;

namespace TagStorage.App.Directory;

public partial class DirectoryContainer : Container<DirectoryItem>
{
    public DirectoryContainer()
    {
        Masking = true;
        CornerRadius = 10;

        Content = new FillFlowContainer<DirectoryItem>
        {
            Direction = FillDirection.Vertical,
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
        };
    }

    protected override FillFlowContainer<DirectoryItem> Content { get; }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
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
                Child = Content,
            }
        ];
    }
}
