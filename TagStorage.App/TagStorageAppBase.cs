using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osuTK;
using TagStorage.Library;
using TagStorage.Library.Facades;
using TagStorage.Library.Installers;
using TagStorage.Resources;

namespace TagStorage.App
{
    public partial class TagStorageAppBase : osu.Framework.Game
    {
        // Anything in this class is shared between the test browser and the game implementation.
        // It allows for caching global dependencies that should be accessible to tests, or changing
        // the screen scaling for all components including the test browser and framework overlays.

        protected override Container<Drawable> Content { get; }

        private DependencyContainer dependencies;

        protected TagStorageAppBase()
        {
            // Ensure game and tests scale with window size and screen DPI.
            base.Content.Add(Content = new DrawSizePreservingFillContainer
            {
                // You may want to change TargetDrawSize to your "default" resolution, which will decide how things scale and position when using absolute coordinates.
                TargetDrawSize = new Vector2(1366, 768)
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Resources.AddStore(new DllResourceStore(TagResources.ResourceAssembly));

            string dbPath = Host.Storage.GetFullPath("tagStorage.db", true);

            dependencies.CacheAs(new DatabaseConnection(dbPath));
            RepositoryInstaller.Install(dependencies);
            FacadeInstaller.Install(dependencies);

            foreach (Type facadeType in typeof(IFacadeBase).Assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(IFacadeBase)) && !t.IsInterface))
            {
                var facade = (IDependencyInjectionCandidate)Activator.CreateInstance(facadeType);
                dependencies.Inject(facade);
                dependencies.Cache(facade);
            }
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
    }
}
