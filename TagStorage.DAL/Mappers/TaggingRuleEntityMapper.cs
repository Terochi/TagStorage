using TagStorage.DAL.Entities;

namespace TagStorage.DAL.Mappers;

public class TaggingRuleEntityMapper : IEntityMapper<TaggingRuleEntity>
{
    public void MapToExistingEntity(TaggingRuleEntity existingEntity, TaggingRuleEntity newEntity)
    {
        existingEntity.Name = newEntity.Name;
    }
}
