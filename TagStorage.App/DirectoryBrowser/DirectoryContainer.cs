#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osuTK;
using TagStorage.App.Selector;
using TagStorage.Library.Entities;
using TagStorage.Library.Helper;
using TagStorage.Library.Repository;

namespace TagStorage.App.DirectoryBrowser;

public partial class DirectoryContainer : CompositeDrawable
{
    [Resolved]
    private TagRepository tags { get; set; }

    [Resolved]
    private ChangeRepository changes { get; set; }

    [Resolved]
    private FileRepository files { get; set; }

    [Resolved]
    private FileLocationRepository fileLocations { get; set; }

    [Resolved]
    private FileTagRepository fileTags { get; set; }

    public override bool RequestsFocus => true;
    public override bool AcceptsFocus => true;

    private TagSearchBox addSearch;
    private TagSearch search;

    public DirectoryContainer()
    {
        Masking = true;
        CornerRadius = 10;
    }

    public DirectorySelectionContainer DirectorySelectionContainer { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            RowDimensions = [new Dimension(GridSizeMode.AutoSize), new Dimension()],
            ColumnDimensions = [new Dimension(GridSizeMode.Relative, 1)],
            Content = new[]
            {
                new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Child = search = new TagSearch()
                    },
                },
                new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children =
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
                            },
                            addSearch = new TagSearchBox
                            {
                                Alpha = 0f,
                                Position = new Vector2(300, 50)
                            }
                        ]
                    }
                }
            }
        };

        DirectorySelectionContainer.AddTag += tags =>
        {
            if (DirectorySelectionContainer.SelectedBlueprints.Count == 0) return;

            addSearch.Show();
        };

        addSearch.OnCommit += onCommit;

        DirectorySelectionContainer.SelectedItems.BindCollectionChanged((_, _) =>
        {
            addSearch.Hide();
        });

        search.SelectedTags.BindCollectionChanged((_, e) =>
        {
            if (search.SelectedTags.Count == 0)
            {
                DirectorySelectionContainer.LoadDirectory(DirectorySelectionContainer.CurrentDirectory.Value);
                return;
            }

            IEnumerable<int> selectedTagIds = search.SelectedTags.Select(t => t.Id);
            IEnumerable<int> taggedFileIds =
                search.SelectedTags.SelectMany(tag => fileTags.GetByTag(tag.Id))
                      .GroupBy(file => file.File)
                      .Where(g => selectedTagIds.All(tag => g.Any(f => f.Tag == tag)))
                      .Select(g => g.Key);

            IEnumerable<FileSystemInfo> fileInfos = taggedFileIds.Select(fileLocations.Get).Where(loc => loc != null).Select(loc =>
            {
                return loc!.Type switch
                {
                    FileLocationType.F => (FileSystemInfo)new FileInfo(loc.Path),
                    FileLocationType.D => new DirectoryInfo(loc.Path),
                    _ => throw new ArgumentOutOfRangeException()
                };
            });
            DirectorySelectionContainer.LoadDirectory(fileInfos);
        });
    }

    public void TagFile(string tagName, string fullname)
    {
        TagEntity? tag = tags.Get(tagName).FirstOrDefault();

        if (tag == null)
        {
            Colour4 newColor = Colour4.FromHSL(Random.Shared.NextSingle(), Random.Shared.NextSingle() * 0.56f + 0.42f, Random.Shared.NextSingle() * 0.5f + 0.4f);
            tag = tags.Insert(new TagEntity { Color = newColor.ToHex(), Name = tagName });
        }

        TagFile(tag, fullname);
    }

    public void TagFile(TagEntity tag, string fullname)
    {
        string machineName = Environment.MachineName;

        string? hash;
        long size;
        FileLocationType type;
        DateTime lastModified;

        if (Directory.Exists(fullname))
        {
            var directoryInfo = new DirectoryInfo(fullname);
            lastModified = DateTime.UtcNow;
            (hash, size) = DirectoryUtils.CreateHash(directoryInfo);
            type = FileLocationType.D;
        }
        else
        {
            var fileInfo = new FileInfo(fullname);
            lastModified = fileInfo.LastWriteTimeUtc;
            (hash, size) = DirectoryUtils.CreateHash(fileInfo);
            type = FileLocationType.F;
        }

        FileEntity file;
        FileLocationEntity? location = fileLocations.GetByPath(fullname).FirstOrDefault(l => l.Machine == machineName);

        if (location == null)
        {
            file = files.Insert(new FileEntity());
            location = fileLocations.Insert(new FileLocationEntity
            {
                File = file.Id,
                Machine = machineName,
                Path = fullname,
                Type = type,
            });
        }
        else if (location.Type != type)
        {
            Logger.Log($"File type does not match with existing {location.Path}", level: LogLevel.Error);
            return;
        }
        else
        {
            file = files.Get(location.File)!;
        }

        IEnumerable<ChangeEntity> foundChanges = changes.FindDuplicates(new ChangeEntity
        {
            Hash = hash,
            Size = size
        });

        bool foundIdentical = false;

        if (foundChanges.Any())
        {
            Logger.Log($"Found duplicate changes for {location.Path}");

            foreach (ChangeEntity change in foundChanges)
            {
                FileLocationEntity loc = fileLocations.Get(change.Location)!;
                Logger.Log($"Duplicate file: {loc.Path}");
                foundIdentical |= change.Location == location.Id;
            }
            // TODO: More handling for matching with already existing FileEntity...
        }

        if (!foundIdentical)
        {
            changes.Insert(new ChangeEntity
            {
                Hash = hash,
                Size = size,
                Date = lastModified,
                Location = location.Id,
            });
        }

        FileTagEntity fileTag = new FileTagEntity
        {
            File = file.Id,
            Tag = tag.Id
        };

        if (!fileTags.Exists(fileTag))
        {
            fileTags.Insert(fileTag);
        }
    }

    private void onCommit(TextBox sender, bool newText)
    {
        TagEntity? tag = tags.Get(sender.Text).FirstOrDefault();

        if (tag == null)
        {
            Colour4 newColor = Colour4.FromHSL(Random.Shared.NextSingle(), Random.Shared.NextSingle() * 0.56f + 0.42f, Random.Shared.NextSingle() * 0.5f + 0.4f);
            tag = tags.Insert(new TagEntity { Color = newColor.ToHex(), Name = sender.Text });
        }

        foreach ((string name, DirectorySelectionItem item) in DirectorySelectionContainer.SelectedItems.Zip(DirectorySelectionContainer.SelectedBlueprints.Cast<DirectorySelectionItem>()))
        {
            string fullName = Path.Join(item.CurrentDirectory.Value, name);
            TagFile(tag, fullName);
        }

        foreach (DirectorySelectionItem selectedBlueprint in DirectorySelectionContainer.SelectedBlueprints.Cast<DirectorySelectionItem>())
        {
            selectedBlueprint.LoadTags();
        }

        addSearch.Hide();
    }

    protected override bool OnClick(ClickEvent e)
    {
        addSearch.Hide();

        return base.OnClick(e);
    }
}
