using System.IO;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace TagStorage.App.Selector;

public partial class DirectorySelectionContainer : SelectionContainer<string>
{
    public string CurrentDirectory { get; private set; } = string.Empty;

    protected override Container<SelectionItem<string>> CreateContent() => new FillFlowContainer<SelectionItem<string>>
    {
        Direction = FillDirection.Vertical,
        AutoSizeAxes = Axes.Both,
    };

    protected override SelectionItem<string> CreateItem(string item) => new DirectorySelectionItem(item);

    protected override bool OnDoubleClick(DoubleClickEvent e)
    {
        var blueprint = SelectedBlueprints.FirstOrDefault(i => i.IsHovered);

        if (blueprint != null)
        {
            LoadDirectory(Path.Join(CurrentDirectory, blueprint.Item));
            return true;
        }

        return base.OnDoubleClick(e);
    }

    public void LoadDirectory(string path)
    {
        CurrentDirectory = path;

        ClearAll();

        string[] directories;
        string[] files;

        if (string.IsNullOrWhiteSpace(path))
        {
            directories = Directory.GetLogicalDrives();
            files = [];
        }
        else
        {
            directories = Directory.GetDirectories(path);
            files = Directory.GetFiles(path);
        }

        foreach (string directory in directories)
        {
            string name = Path.GetFileName(directory);
            if (string.IsNullOrWhiteSpace(name)) name = directory;
            AddBlueprintFor(name);
        }

        foreach (string file in files)
        {
            string name = Path.GetFileName(file);
            AddBlueprintFor(name);
        }
    }
}
