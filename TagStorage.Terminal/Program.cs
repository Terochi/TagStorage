using TagStorage.Library;
using TagStorage.Library.Helper;

namespace TagStorage.Terminal;

public static class Program
{
    public static void PrintGraphViz(DatabaseConnection db)
    {
        Console.WriteLine("digraph G {");

        foreach ((int id, string label, int? parentId, string? parentName) in
                 db.ExecuteQuery<Tuple<int, string, int?, string?>>(
                     """
                     SELECT children.id, children.name, parents.id, parents.name
                     FROM tag_children
                         LEFT JOIN tags children ON tag_children.child = children.id
                         LEFT JOIN tags parents ON tag_children.parent = parents.id
                     """,
                     r =>
                     {
                         int id = r.GetInt32(0);
                         string label = r.GetString(1);
                         int? parentId = r.IsDBNull(2) ? null : r.GetInt32(2);
                         string? parentName = r.IsDBNull(3) ? null : r.GetString(3);
                         return new Tuple<int, string, int?, string?>(id, label, parentId, parentName);
                     }))
        {
            if (parentId.HasValue)
            {
                Console.WriteLine($"\tn{parentId.Value} -> n{id};");
                Console.WriteLine($"\tn{parentId.Value} [label=\"{parentName}\"];");
            }

            Console.WriteLine($"\tn{id} [label=\"{label}\"];");
        }

        Console.WriteLine("}");
    }

    public static void Main(string[] args)
    {
        Console.Write("Choose dir: ");
        string dir = Console.ReadLine() ?? string.Empty;

        if (Directory.Exists(dir))
        {
            var footprint = DirectoryUtils.CreateHash(new DirectoryInfo(dir));
            Console.WriteLine(footprint);
        }
    }
}
