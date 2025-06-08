using Microsoft.EntityFrameworkCore;
using TagStorage.DAL;
using TagStorage.DAL.Entities;
using TagStorage.DAL.Factories;
using TagStorage.DAL.Mappers;
using TagStorage.DAL.UnitOfWork;

namespace TagStorage.Tests;

public class DALTest
{
    [Test]
    public async Task TestDatabaseConnection()
    {
        // Arrange
        IDbContextFactory<TagStorageDbContext> factory = new DbContextSqliteFactory(GetType().FullName!);

        await using var dbx = await factory.CreateDbContextAsync();
        var uowFactory = new UnitOfWork(dbx);
        await dbx.Database.EnsureDeletedAsync();
        await dbx.Database.EnsureCreatedAsync();
        await dbx.SaveChangesAsync();

        var tagRepository = uowFactory.GetRepository<TagEntity, TagEntityMapper>();
        var fileTagRepository = uowFactory.GetRepository<FileTagEntity, FileTagEntityMapper>();
        var fileRepository = uowFactory.GetRepository<FileEntity, FileEntityMapper>();
        var fileLocationRepository = uowFactory.GetRepository<FileLocationEntity, FileLocationEntityMapper>();

        var tagEntity = new TagEntity
        {
            Id = Guid.NewGuid(),
            Name = "TestTag",
            Color = "FF0000",
        };
        tagRepository.Insert(tagEntity);
        var fileEntity = new FileEntity
        {
            Id = Guid.NewGuid(),
        };
        fileRepository.Insert(fileEntity);
        var fileLocationEntity = new FileLocationEntity
        {
            Id = Guid.NewGuid(),
            FileId = fileEntity.Id,
            Path = "C:\\Test\\File.txt",
            Machine = "TestMachine",
        };
        fileLocationRepository.Insert(fileLocationEntity);
        var fileTagEntity = new FileTagEntity
        {
            FileId = fileEntity.Id,
            TagId = tagEntity.Id,
        };
        fileTagRepository.Insert(fileTagEntity);

        await uowFactory.CommitAsync();

        Assert.That(await tagRepository.ExistsAsync(tagEntity));
    }
}
