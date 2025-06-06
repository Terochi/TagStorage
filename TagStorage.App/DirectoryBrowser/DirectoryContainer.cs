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
using osuTK;
using TagStorage.App.Selector;
using TagStorage.Library.Entities;
using TagStorage.Library.Facades;

namespace TagStorage.App.DirectoryBrowser;

public partial class DirectoryContainer : CompositeDrawable
{
    [Resolved]
    private TagFacade tags { get; set; }

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

            IEnumerable<FileSystemInfo> fileInfos = tags.GetFileSelectedTags(search.SelectedTags).Select(loc =>
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

    private void onCommit(TextBox sender, bool newText)
    {
        TagEntity tag = tags.Get(sender.Text).FirstOrDefault() ??
                        tags.Insert(sender.Text);

        foreach ((string name, DirectorySelectionItem item) in DirectorySelectionContainer.SelectedItems.Zip(DirectorySelectionContainer.SelectedBlueprints.Cast<DirectorySelectionItem>()))
        {
            string fullName = Path.Join(item.CurrentDirectory.Value, name);
            tags.TagFile(tag, fullName);
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
