using TagStorage.DAL.Entities;

namespace TagStorage.DAL.Mappers;

public class FileLocationEntityMapper : IEntityMapper<FileLocationEntity>
{
    public void MapToExistingEntity(FileLocationEntity existingEntity, FileLocationEntity newEntity)
    {
        existingEntity.Id = newEntity.Id;
        existingEntity.Type = newEntity.Type;
        existingEntity.Path = newEntity.Path;
        existingEntity.Machine = newEntity.Machine;
    }
}
