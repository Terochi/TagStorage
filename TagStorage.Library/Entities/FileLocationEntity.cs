namespace TagStorage.Library.Entities;

public class FileLocationEntity : IEntity
{
    public int Id { get; set; }
    public int File { get; set; }

    public FileLocationType Type { get; set; }
    public required string Path { get; set; }

    public required string Machine { get; set; }
}

public enum FileLocationType
{
    F, // File
    D, // Directory
}
