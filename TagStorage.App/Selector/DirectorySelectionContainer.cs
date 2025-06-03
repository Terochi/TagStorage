using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NaturalSort.Extension;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace TagStorage.App.Selector;

public partial class DirectorySelectionContainer : SelectionContainer<string>
{
    public Bindable<string> CurrentDirectory { get; private set; } = new Bindable<string>(string.Empty);

    public event Action<IEnumerable<string>> AddTag;

    protected override Container<SelectionItem<string>> CreateContent() => new FillFlowContainer<SelectionItem<string>>
    {
        Direction = FillDirection.Vertical,
        AutoSizeAxes = Axes.Both,
    };

    protected override SelectionItem<string> CreateItem(string item)
    {
        if (Directory.Exists(Path.Join(CurrentDirectory.Value, item)))
            return new DirectorySelectionItem(item) { CurrentDirectory = CurrentDirectory.GetBoundCopy() };

        return new FileSelectionItem(item) { CurrentDirectory = CurrentDirectory.GetBoundCopy() };
    }

    protected override bool OnDoubleClick(DoubleClickEvent e)
    {
        var blueprint = SelectedBlueprints.FirstOrDefault(i => i.IsHovered);

        if (blueprint is DirectorySelectionItem and not FileSelectionItem)
        {
            LoadDirectory(Path.Join(CurrentDirectory.Value, blueprint.Item));
            return true;
        }

        return base.OnDoubleClick(e);
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key == Key.A)
        {
            AddTag?.Invoke(SelectedItems.AsEnumerable());

            return true;
        }

        return base.OnKeyDown(e);
    }

    public void LoadDirectory(string path)
    {
        CurrentDirectory.Value = path;

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

        foreach (string directory in directories.OrderBy(Path.GetFileNameWithoutExtension, new NaturalSortComparer(StringComparer.OrdinalIgnoreCase)))
        {
            string name = Path.GetFileName(directory);
            if (string.IsNullOrWhiteSpace(name)) name = directory;
            AddBlueprintFor(name);
        }

        foreach (string file in files.OrderBy(Path.GetFileNameWithoutExtension, new NaturalSortComparer(StringComparer.OrdinalIgnoreCase)))
        {
            string name = Path.GetFileName(file);
            AddBlueprintFor(name);
        }
    }
}
