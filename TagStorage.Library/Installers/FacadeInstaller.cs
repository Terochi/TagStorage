using osu.Framework.Allocation;
using TagStorage.Library.Facades;

namespace TagStorage.Library.Installers;

public static class FacadeInstaller
{
    public static void Install(DependencyContainer dependencies)
    {
        foreach (Type facadeType in typeof(IFacadeBase).Assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(IFacadeBase)) && !t.IsInterface))
        {
            var facade = (IDependencyInjectionCandidate)Activator.CreateInstance(facadeType)!;
            dependencies.Inject(facade);
            dependencies.Cache(facade);
        }
    }
}
