using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Input;
using TagStorage.App.DirectoryBrowser;

namespace TagStorage.App
{
    public partial class MainScreen : Screen
    {
        private DirectoryContainer dir;

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (e.Key == Key.BackSpace)
            {
                if (string.IsNullOrWhiteSpace(dir.DirectorySelectionContainer.CurrentDirectory))
                    return true;

                var directoryInfo = Directory.GetParent(dir.DirectorySelectionContainer.CurrentDirectory);

                dir.DirectorySelectionContainer.LoadDirectory(directoryInfo != null ? directoryInfo.FullName : "");

                return true;
            }

            return base.OnKeyDown(e);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = dir = new DirectoryContainer()
            {
                RelativeSizeAxes = Axes.Both,
            };
        }

        protected override void LoadComplete()
        {
            dir.DirectorySelectionContainer.LoadDirectory(dir.DirectorySelectionContainer.CurrentDirectory);

            base.LoadComplete();
        }
    }
}
