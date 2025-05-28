using NUnit.Framework;
using osu.Framework.Graphics;
using osuTK;
using TagStorage.App.Directory;

namespace TagStorage.App.Tests.Visual
{
    [TestFixture]
    public partial class TestSceneDirectory : TagStorageTestScene
    {
        // Add visual tests to ensure correct behaviour of your game: https://github.com/ppy/osu-framework/wiki/Development-and-Testing
        // You can make changes to classes associated with the tests and they will recompile and update immediately.

        public TestSceneDirectory()
        {
            var directoryContainer = new DirectoryContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(400, 300),
            };

            Add(directoryContainer);

            AddRepeatStep("Add Directory Item", () =>
            {
                directoryContainer.Add(new DirectoryItem
                {
                    Text = "Test Item " + directoryContainer.Children.Count,
                });
            }, 20);
            AddStep("Clear Directory Items", () =>
            {
                directoryContainer.Clear();
            });
        }
    }
}
