using JetBrains.Annotations;

namespace TagStorage.Library.Entities;

public class ChangeEntity : IEntity
{
    public int Id { get; set; }
    public int Location { get; set; }
    public DateTime Date { get; set; }
    public long Size { get; set; }

    [CanBeNull]
    public string Hash { get; set; }
}
