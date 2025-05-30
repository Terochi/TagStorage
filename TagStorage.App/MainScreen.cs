using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Input;
using TagStorage.App.DirectoryBrowser;

namespace TagStorage.App
{
    public partial class MainScreen : Screen
    {
        private DirectoryContainer dir;

        private string path = "";

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (e.Key == Key.BackSpace)
            {
                if (string.IsNullOrWhiteSpace(path))
                    return true;

                var directoryInfo = Directory.GetParent(path);

                loadDirectory(directoryInfo != null ? directoryInfo.FullName : "");

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

            loadDirectory(path);
        }

        private void loadDirectory(string path)
        {
            dir.Clear();
            this.path = path;

            string[] directories;
            string[] files;

            if (string.IsNullOrWhiteSpace(path))
            {
                directories = Directory.GetLogicalDrives();
                files = [];
            }
            else
            {
                directories = Directory.GetDirectories(path);
                files = Directory.GetFiles(path);
            }

            foreach (string directory in directories)
            {
                string name = Path.GetFileName(directory);
                if (string.IsNullOrWhiteSpace(name)) name = directory;
                dir.Add(new DirectoryItem
                {
                    Text = name,
                    Icon = FontAwesome.Solid.Folder,
                    Colour = Colour4.FromHex("FFE99A"),
                    Action = () =>
                    {
                        loadDirectory(string.IsNullOrWhiteSpace(path) ? name : Path.Combine(path, name));
                    }
                });
            }

            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                dir.Add(new DirectoryItem
                {
                    Text = name,
                    Icon = FontAwesome.Solid.File,
                    Colour = new Colour4(0.4f, 0.7f, 0.9f, 1f),
                });
            }
        }
    }
}
