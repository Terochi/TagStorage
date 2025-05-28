using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Input;
using TagStorage.App.Directory;

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
                var directoryInfo = System.IO.Directory.GetParent(path);

                if (directoryInfo != null)
                {
                    loadDirectory(directoryInfo.FullName);
                }
                else
                {
                    loadDirectory("");
                }

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
                directories = System.IO.Directory.GetLogicalDrives();
                files = [];
            }
            else
            {
                directories = System.IO.Directory.GetDirectories(path);
                files = System.IO.Directory.GetFiles(path);
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
                        if (string.IsNullOrWhiteSpace(path))
                        {
                            loadDirectory(name);
                        }
                        else
                        {
                            loadDirectory(Path.Combine(path, name));
                        }
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
