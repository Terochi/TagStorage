using TagStorage.DAL.Entities;

namespace TagStorage.DAL.Mappers;

public class ChangeEntityMapper : IEntityMapper<ChangeEntity>
{
    public void MapToExistingEntity(ChangeEntity existingEntity, ChangeEntity newEntity)
    {
        existingEntity.Date = newEntity.Date;
        existingEntity.Size = newEntity.Size;
        existingEntity.Hash = newEntity.Hash;
    }
}
