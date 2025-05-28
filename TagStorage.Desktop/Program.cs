using osu.Framework;
using osu.Framework.Platform;

namespace TagStorage.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost(@"Tag"))
            using (Game game = new App.TagStorageApp())
                host.Run(game);
        }
    }
}
