using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using TagStorage.App.TagBrowser;

namespace TagStorage.App;

public partial class TagScreen : Screen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new TagHierarchy
        {
            RelativeSizeAxes = Axes.Both,
        };
    }
}
