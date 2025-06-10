using TagStorage.DAL.Entities;

namespace TagStorage.DAL.Mappers;

public class DirectoryEntityMapper : IEntityMapper<DirectoryEntity>
{
    public void MapToExistingEntity(DirectoryEntity existingEntity, DirectoryEntity newEntity)
    {
        existingEntity.Type = newEntity.Type;
        existingEntity.Directory = newEntity.Directory;
    }
}
