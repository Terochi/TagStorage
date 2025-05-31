using TagStorage.Library;
using TagStorage.Library.Entities;

namespace TagStorage.Terminal;

public static class Program
{
    public static TagEntity? SelectTag(TagRepository repository, string prompt = "Enter tag: ")
    {
        var interactiveInput = new InteractiveInput<TagEntity>(
            repository.Get,
            printTransform,
            prompt);

        return interactiveInput.ReadInput();
    }

    private static string printTransform(TagEntity t)
    {
        return t.Name;
    }

    public static void PrintGraphViz(DatabaseConnection db)
    {
        Console.WriteLine("digraph G {");

        foreach ((int id, string label, int? parentId, string? parentName) in
                 db.ExecuteQuery<Tuple<int, string, int?, string?>>(
                     """
                     SELECT children.id, children.name, parents.id, parents.name
                     FROM tag_parent_of
                         LEFT JOIN tags children ON tag_parent_of.child = children.id
                         LEFT JOIN tags parents ON tag_parent_of.parent = parents.id
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
        var db = new DatabaseConnection("tagStorage.db");
        var tagRepository = new TagRepository(db);

        while (true)
        {
            Console.WriteLine("'A' to add a tag");
            Console.WriteLine("'F' to find a tag");
            Console.WriteLine("'L' to link two tags");
            Console.WriteLine("'P' to print diagraph");
            Console.WriteLine("'C' to print child tags of a tag");
            Console.WriteLine("'N' to print nested tags of a tag");
            Console.WriteLine("'Q' to quit.");

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Q)
            {
                break;
            }

            if (key.Key == ConsoleKey.A)
            {
                Console.Write("Enter tag name: ");
                string tagName = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrEmpty(tagName))
                {
                    continue;
                }

                TagEntity child = tagRepository.Add(tagName);
                Console.WriteLine($"Tag '{child.Name}' added.");

                TagEntity? parent = SelectTag(tagRepository, "Select parent tag: ");

                if (parent != null)
                {
                    tagRepository.Link(child.Id, parent.Id);
                    Console.WriteLine($"Tag '{child.Name}' added as a child of '{parent.Name}'.");
                }
                else
                {
                    Console.WriteLine("No parent tag selected.");
                }
            }

            if (key.Key == ConsoleKey.L)
            {
                TagEntity? child = SelectTag(tagRepository, "Select child tag: ");
                TagEntity? parent = SelectTag(tagRepository, "Select parent tag: ");

                if (parent != null && child != null)
                {
                    tagRepository.Link(child.Id, parent.Id);
                    Console.WriteLine($"Tag '{child.Name}' added as a child of '{parent.Name}'.");
                }
                else
                {
                    Console.WriteLine("Empty tag selected.");
                }
            }

            if (key.Key == ConsoleKey.F)
            {
                TagEntity? tag = SelectTag(tagRepository);

                Console.WriteLine(tag == null ? "No tag found." : $"Found tag: {tag.Name}");
            }

            if (key.Key == ConsoleKey.C)
            {
                TagEntity? parent = SelectTag(tagRepository, "Select parent tag: ");

                if (parent != null)
                {
                    Console.WriteLine($"Child tags of '{parent.Name}':");

                    foreach (TagEntity child in tagRepository.GetChildTags(parent.Id))
                    {
                        Console.WriteLine($"- {child.Name}");
                    }
                }
                else
                {
                    Console.WriteLine("No tag selected.");
                }
            }

            if (key.Key == ConsoleKey.N)
            {
                TagEntity? parent = SelectTag(tagRepository, "Select parent tag: ");

                if (parent != null)
                {
                    Console.WriteLine($"Child tags of '{parent.Name}':");

                    foreach (TagEntity child in tagRepository.GetNestedChildTags(parent.Id))
                    {
                        Console.WriteLine($"- {child.Name}");
                    }
                }
                else
                {
                    Console.WriteLine("No tag selected.");
                }
            }

            if (key.Key == ConsoleKey.P)
            {
                Console.Clear();
                PrintGraphViz(db);
            }
        }
    }
}
