namespace TagStorage.Library.Entities;

public class TagChildEntity : IEntity
{
    public int Id => (Child << 16) | Parent;
    public int Child { get; set; }
    public int Parent { get; set; }
}
