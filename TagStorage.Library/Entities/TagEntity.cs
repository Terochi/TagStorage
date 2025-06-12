using JetBrains.Annotations;

namespace TagStorage.Library.Entities;

public class TagEntity : IEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }

    [CanBeNull]
    public string Color { get; set; }
}
