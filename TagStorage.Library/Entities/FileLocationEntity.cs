using System.ComponentModel;

namespace TagStorage.Library.Entities;

public class FileLocationEntity : IEntity
{
    public int Id { get; set; }

    public FileLocationType Type { get; set; }
    public required string Path { get; set; }

    public required string Machine { get; set; }
}

public enum FileLocationType
{
    [Description("F")]
    File,

    [Description("D")]
    Directory,
}
