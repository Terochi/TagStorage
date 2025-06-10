using TagStorage.DAL.Entities;

namespace TagStorage.DAL.Mappers;

public class TagEntityMapper : IEntityMapper<TagEntity>
{
    public void MapToExistingEntity(TagEntity existingEntity, TagEntity newEntity)
    {
        existingEntity.Name = newEntity.Name;
        existingEntity.Color = newEntity.Color;
    }
}
