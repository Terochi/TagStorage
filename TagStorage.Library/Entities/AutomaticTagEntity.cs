namespace TagStorage.Library.Entities;

public class AutomaticTagEntity : IEntity
{
    public int Id { get; set; }
    public int Rule { get; set; }
    public required string Directory { get; set; }
}
