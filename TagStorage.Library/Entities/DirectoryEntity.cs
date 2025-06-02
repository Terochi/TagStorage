using System.ComponentModel;

namespace TagStorage.Library.Entities;

public class DirectoryEntity : IEntity
{
    public int Id { get; set; }
    public DirectoryType Type { get; set; }
    public required string Directory { get; set; }
}

public enum DirectoryType
{
    [Description("I")]
    Included,

    [Description("E")]
    Excluded,
}
