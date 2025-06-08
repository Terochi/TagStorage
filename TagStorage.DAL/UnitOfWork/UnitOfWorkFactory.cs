using Microsoft.EntityFrameworkCore;

namespace TagStorage.DAL.UnitOfWork;

public class UnitOfWorkFactory(IDbContextFactory<TagStorageDbContext> dbContextFactory)
{
    public UnitOfWork Create() => new UnitOfWork(dbContextFactory.CreateDbContext());
}
