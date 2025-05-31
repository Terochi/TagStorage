using osu.Framework;
using osu.Framework.Platform;

namespace TagStorage.App.Tests
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost("TagStorage-tests"))
            using (var game = new TagStorageTestBrowser())
                host.Run(game);
        }
    }
}
