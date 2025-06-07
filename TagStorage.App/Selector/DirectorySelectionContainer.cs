using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NaturalSort.Extension;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK.Input;
using TagStorage.App.DirectoryBrowser;
using TagStorage.Library.Entities;
using TagStorage.Library.Facades;

namespace TagStorage.App.Selector;

public partial class DirectorySelectionContainer : SelectionContainer<string>
{
    [Resolved]
    private TagFacade tags { get; set; }

    [Resolved]
    private AutoTagFacade autoTags { get; set; }

    public Bindable<string> CurrentDirectory { get; private set; } = new Bindable<string>(string.Empty);

    public event Action<IEnumerable<string>> AddTag;

    protected override Container<SelectionItem<string>> CreateContent() => new FillFlowContainer<SelectionItem<string>>
    {
        Direction = FillDirection.Vertical,
        AutoSizeAxes = Axes.Both,
    };

    protected override SelectionItem<string> CreateItem(string item)
    {
        var directoryInfo = new DirectoryInfo(item);
        var fileInfo = new FileInfo(item);

        if (directoryInfo.Exists && !directoryInfo.FullName.EndsWith(":\\"))
        {
            var selectionItem = new DirectorySelectionItem(directoryInfo.Name);
            selectionItem.CurrentDirectory.Value = directoryInfo.Parent!.FullName;
            return selectionItem;
        }

        if (fileInfo.Exists)
        {
            var selectionItem = new FileSelectionItem(directoryInfo.Name);
            selectionItem.CurrentDirectory.Value = fileInfo.DirectoryName;
            return selectionItem;
        }

        if (Directory.Exists(Path.Join(CurrentDirectory.Value, item)))
            return new DirectorySelectionItem(item) { CurrentDirectory = CurrentDirectory.GetBoundCopy() };

        return new FileSelectionItem(item) { CurrentDirectory = CurrentDirectory.GetBoundCopy() };
    }

    protected override bool OnDoubleClick(DoubleClickEvent e)
    {
        var blueprint = SelectedBlueprints.FirstOrDefault(i => i.IsHovered);

        if (blueprint is DirectorySelectionItem directoryItem and not FileSelectionItem)
        {
            LoadDirectory(Path.Join(directoryItem.CurrentDirectory.Value, directoryItem.Item));
            return true;
        }

        return base.OnDoubleClick(e);
    }

    private readonly Regex wordSplitRegex = new Regex(@"(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])|(?<=[A-Za-z])(?=\d)|[& \t\-_]+|(?<=\d)(?=[A-Za-z])", RegexOptions.Compiled);

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key == Key.A)
        {
            AddTag?.Invoke(SelectedItems.AsEnumerable());

            return true;
        }

        if (e.Key == Key.D)
        {
            TaggingRuleEntity rule = autoTags.GetTagRules().First();

            if (autoTags.Get(CurrentDirectory.Value).Any(a => a.Rule == rule.Id))
            {
                return true;
            }

            autoTags.Insert(new AutomaticTagEntity
            {
                Directory = CurrentDirectory.Value,
                Rule = rule.Id
            });

            Drawable parent = Parent;

            while (parent != null && parent is not DirectoryContainer)
            {
                parent = parent.Parent;
            }

            if (parent is DirectoryContainer container)
            {
                var curDir = new DirectoryInfo(CurrentDirectory.Value);
                DirectoryInfo[] subDirectories = curDir.GetDirectories();
                FileInfo[] files = curDir.GetFiles();

                foreach (DirectoryInfo dir in subDirectories)
                {
                    foreach (string name in names(Path.GetFileNameWithoutExtension(dir.FullName)))
                    {
                        tags.TagFile(name.ToLowerInvariant(), dir.FullName);
                    }
                }

                foreach (FileInfo file in files)
                {
                    foreach (string name in names(Path.GetFileNameWithoutExtension(file.FullName)))
                    {
                        tags.TagFile(name.ToLowerInvariant(), file.FullName);
                    }
                }

                LoadDirectory(CurrentDirectory.Value);

                IEnumerable<string> names(string name)
                {
                    return wordSplitRegex.Split(name);
                }
            }

            return true;
        }

        return base.OnKeyDown(e);
    }

    public void LoadDirectory(IEnumerable<FileSystemInfo> files)
    {
        ClearAll();

        foreach (FileSystemInfo file in files.OrderBy(f => Path.GetFileNameWithoutExtension(f.FullName), new NaturalSortComparer(StringComparer.OrdinalIgnoreCase)))
        {
            AddBlueprintFor(file.FullName);
        }
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
