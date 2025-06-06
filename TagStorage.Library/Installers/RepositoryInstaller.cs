using osu.Framework.Allocation;
using TagStorage.Library.Repositories;

namespace TagStorage.Library.Installers;

public static class RepositoryInstaller
{
    public static void Install(DependencyContainer dependencies)
    {
        foreach (Type repositoryType in typeof(BaseRepository<>).Assembly.GetTypes().Where(t => t.BaseType?.Name == typeof(BaseRepository<>).Name))
        {
            var repository = (IDependencyInjectionCandidate)Activator.CreateInstance(repositoryType)!;
            dependencies.Inject(repository);
            dependencies.Cache(repository);
        }
    }
}
