namespace TagStorage.Library.Entities;

public class FileTagEntity : IEntity
{
    public int Id => (Tag << 16) | File;
    public int Tag { get; set; }
    public int File { get; set; }
}
