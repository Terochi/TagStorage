namespace TagStorage.Library.Entities;

public class ChangeEntity : IEntity
{
    public int Id { get; set; }
    public int Location { get; set; }
    public DateTime Date { get; set; }
    public int Size { get; set; }
    public required string Hash { get; set; }
}
