using TagStorage.DAL.Entities;

namespace TagStorage.DAL.Mappers;

public class AutoTagEntityMapper : IEntityMapper<AutoTagEntity>
{
    public void MapToExistingEntity(AutoTagEntity existingEntity, AutoTagEntity newEntity)
    {
        existingEntity.Directory = newEntity.Directory;
    }
}
