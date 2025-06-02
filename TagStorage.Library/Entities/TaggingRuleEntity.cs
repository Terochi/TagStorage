namespace TagStorage.Library.Entities;

public class TaggingRuleEntity : IEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
