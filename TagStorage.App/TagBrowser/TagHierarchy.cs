using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Graphics.Shapes;
using osuTK;
using TagStorage.Library.Entities;
using TagStorage.Library.Facades;

namespace TagStorage.App.TagBrowser;

public partial class TagHierarchy : CompositeDrawable
{
    private TagFacade tags;

    private TagHierarchyManager hierarchyManager;

    private Vector2 drawSize = new Vector2();

    [BackgroundDependencyLoader]
    private void load(TagFacade tags)
    {
        this.tags = tags;

        List<GraphNode> nodes = new List<GraphNode>();

        foreach (TagEntity tag in tags.Tags)
        {
            nodes.Add(new GraphNode(tag, tags, nodes));
        }

        hierarchyManager = new TagHierarchyManager(tags, nodes);

        hierarchyManager.AssignRanks();
        hierarchyManager.MinimizeCrossings();
    }

    [CanBeNull]
    private Tag lastClickedTag = null;

    protected override void Update()
    {
        base.Update();

        if (drawSize == DrawSize)
            return;

        drawSize = DrawSize;

        List<Tag> tagDrawables = new List<Tag>();
        List<Path> pathDrawables = new List<Path>();

        IEnumerable<IGrouping<int, GraphNode>> nodeRanks = hierarchyManager.Nodes.GroupBy(n => n.Rank);
        float rankCount = nodeRanks.Count();

        foreach (IGrouping<int, GraphNode> rankedNodes in nodeRanks)
        {
            float orderCount = rankedNodes.Count();

            foreach (var node in rankedNodes)
            {
                Tag tag = new Tag(node.Tag)
                {
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.Centre,
                    X = (node.Order / orderCount * 0.8f + 0.1f) * DrawWidth,
                    Y = (node.Rank / rankCount * 0.8f + 0.1f) * DrawHeight,
                };

                foreach (GraphNode nodeParent in node.Parents)
                {
                    float x = (nodeParent.Order /
                        (float)nodeRanks.First(n => n.Key == nodeParent.Rank).Count()
                        * 0.8f + 0.1f) * DrawWidth;
                    float y = (nodeParent.Rank / rankCount * 0.8f + 0.1f) * DrawHeight;

                    Path path = new SmoothPath
                    {
                        // RelativeSizeAxes = Axes.Both,
                        Vertices = [new Vector2(x, y) + Vector2.UnitY * 10, tag.Position - Vector2.UnitY * 10],
                        PathRadius = 1f,
                        Colour = Colour4.Black,
                    };
                    pathDrawables.Add(path);
                }

                tag.Clicked += onTagOnClicked;
                tagDrawables.Add(tag);
            }
        }

        InternalChildren =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.DarkGray,
            },
            ..tagDrawables,
            ..pathDrawables
        ];
    }

    private void onTagOnClicked(Tag t)
    {
        if (lastClickedTag == null)
        {
            lastClickedTag = t;
            return;
        }

        if (!tags.LinkTags(t.Entity, lastClickedTag.Entity))
        {
            tags.UnlinkTags(t.Entity, lastClickedTag.Entity);
        }

        lastClickedTag = null;

        foreach (GraphNode node in hierarchyManager.Nodes)
        {
            node.DiscardHierarchy();
        }

        hierarchyManager.AssignRanks();
        hierarchyManager.MinimizeCrossings();

        drawSize = new Vector2();
    }
}
