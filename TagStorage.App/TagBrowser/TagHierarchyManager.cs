using System.Collections.Generic;
using System.Linq;
using TagStorage.Library.Entities;
using TagStorage.Library.Facades;

namespace TagStorage.App.TagBrowser;

public class GraphNode(TagEntity tag, TagFacade tags, List<GraphNode> nodes)
{
    public readonly TagEntity Tag = tag;
    private readonly TagFacade tags = tags;
    private readonly List<GraphNode> nodes = nodes;

    public int Id => Tag.Id;

    private List<GraphNode> parents;
    private List<GraphNode> children;
    public List<GraphNode> Parents => parents ??= getNodes(tags.GetParents(Tag));
    public List<GraphNode> Children => children ??= getNodes(tags.GetChildren(Tag));

    public int Rank = int.MaxValue - 1;
    public int Order;

    private List<GraphNode> getNodes(IEnumerable<TagEntity> tagEntities)
    {
        return tagEntities.Select(t =>
        {
            var node = nodes.FirstOrDefault(n => n.Id == t.Id);

            if (node == null)
            {
                node = new GraphNode(t, tags, nodes);
                nodes.Add(node);
            }

            return node;
        }).ToList();
    }

    public void DiscardHierarchy()
    {
        parents = null;
        children = null;
        Rank = int.MaxValue - 1;
    }
}

public class TagHierarchyManager(TagFacade tags, List<GraphNode> nodes)
{
    private TagFacade tags = tags;

    public readonly List<GraphNode> Nodes = nodes;

    public void AssignRanks()
    {
        // Simple topological sort with rank = longest path from root
        var inDegree = Nodes.ToDictionary(n => n, n => n.Parents.Count);
        Queue<GraphNode> queue = new Queue<GraphNode>();

        foreach (var node in Nodes)
        {
            if (inDegree[node] == 0)
            {
                queue.Enqueue(node);
                node.Rank = 0;
            }
        }

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            foreach (var child in node.Children)
            {
                if (child.Rank > node.Rank + 1)
                {
                    child.Rank = node.Rank + 1;
                    queue.Enqueue(child);
                }
            }
        }

        foreach (GraphNode node in Nodes.Where(n => n.Rank == int.MaxValue - 1))
        {
            node.Rank = 0;
        }
    }

    public void MinimizeCrossings(int maxIter = 10)
    {
        // Group nodes by rank
        var ranks = Nodes.GroupBy(n => n.Rank)
                         .OrderBy(g => g.Key)
                         .ToDictionary(g => g.Key, g => g.OrderBy(n => n.Id).ToList());

        // Initialize order
        foreach (var kv in ranks)
        {
            int i = 0;
            foreach (var node in kv.Value)
                node.Order = i++;
        }

        for (int iter = 0; iter < maxIter; iter++)
        {
            // Downward sweep
            for (int r = 1; ranks.ContainsKey(r); r++)
            {
                applyMedian(ranks[r], ranks[r - 1], upward: false);
            }

            // Upward sweep
            for (int r = ranks.Keys.Max() - 1; r >= 0; r--)
            {
                applyMedian(ranks[r], ranks[r + 1], upward: true);
            }

            // Transpose
            foreach (var rank in ranks.Values)
            {
                bool improved;

                do
                {
                    improved = false;

                    for (int i = 0; i < rank.Count - 1; i++)
                    {
                        var n1 = rank[i];
                        var n2 = rank[i + 1];

                        if (countCrossings(n1, n2) > countCrossings(n2, n1))
                        {
                            // Swap
                            rank[i] = n2;
                            rank[i + 1] = n1;
                            n1.Order = i + 1;
                            n2.Order = i;
                            improved = true;
                        }
                    }
                } while (improved);
            }
        }
    }

    private void applyMedian(List<GraphNode> rank, List<GraphNode> adjacentRank, bool upward)
    {
        foreach (var node in rank)
        {
            var neighbors = upward ? node.Children : node.Parents;
            var positions = neighbors.Select(n => n.Order).OrderBy(x => x).ToList();
            node.Order = positions.Count == 0 ? node.Order : positions[positions.Count / 2]; // median
        }

        rank.Sort((a, b) => a.Order.CompareTo(b.Order));
        for (int i = 0; i < rank.Count; i++) rank[i].Order = i;
    }

    private int countCrossings(GraphNode left, GraphNode right)
    {
        int crossings = 0;

        foreach (var a in left.Children)
        {
            foreach (var b in right.Children)
            {
                if (b.Id == left.Id)
                    continue;

                if (a.Order > b.Order)
                    crossings++;
            }
        }

        return crossings;
    }
}
