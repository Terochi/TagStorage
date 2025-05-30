using NUnit.Framework;
using osu.Framework.Graphics;
using osuTK;
using TagStorage.App.TagBrowser;

namespace TagStorage.App.Tests.Visual
{
    [TestFixture]
    public partial class TestSceneTagBrowser : TagStorageTestScene
    {
        // Add visual tests to ensure correct behaviour of your game: https://github.com/ppy/osu-framework/wiki/Development-and-Testing
        // You can make changes to classes associated with the tests and they will recompile and update immediately.

        public TestSceneTagBrowser()
        {
            Add(new TagList()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(400, 300),
            });
        }
    }
}
