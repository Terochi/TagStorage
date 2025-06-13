using NUnit.Framework;
using osu.Framework.Graphics;
using osuTK;
using TagStorage.App.TagBrowser;
using TagStorage.Library.Entities;

namespace TagStorage.App.Tests.Visual
{
    [TestFixture]
    public partial class TestSceneTagBrowser : TagStorageTestScene
    {
        // Add visual tests to ensure correct behaviour of your game: https://github.com/ppy/osu-framework/wiki/Development-and-Testing
        // You can make changes to classes associated with the tests and they will recompile and update immediately.

        public TestSceneTagBrowser()
        {
            var tagList = new TagList()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(400, 300),
            };
            tagList.LoadTags([
                new TagEntity { Id = 1, Name = "Tag 1", Color = "#FF0000" },
                new TagEntity { Id = 2, Name = "Tag 2", Color = "#00FF00" },
                new TagEntity { Id = 3, Name = "Tag 3", Color = "#0000FF" },
                new TagEntity { Id = 4, Name = "Tag 4", Color = "#FFFF00" },
                new TagEntity { Id = 5, Name = "Tag 5", Color = "#FF00FF" },
                new TagEntity { Id = 6, Name = "Tag 6", Color = "#00FFFF" },
            ]);
            Add(tagList);
        }
    }
}
