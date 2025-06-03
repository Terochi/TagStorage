using osu.Framework.Graphics.Sprites;

namespace TagStorage.App.Selector;

public partial class FileSelectionItem(string item) : DirectorySelectionItem(item)
{
    protected override IconUsage GetIcon => FontAwesome.Solid.File;
}
