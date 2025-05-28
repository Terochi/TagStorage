using osu.Framework;
using osu.Framework.Platform;
using TagStorage.App;

namespace TagStorage.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost(@"Tag"))
            using (osu.Framework.Game game = new TagGame())
                host.Run(game);
        }
    }
}
