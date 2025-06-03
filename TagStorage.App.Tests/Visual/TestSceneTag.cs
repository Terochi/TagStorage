using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using TagStorage.App.TagBrowser;
using TagStorage.Library.Entities;

namespace TagStorage.App.Tests.Visual
{
    [TestFixture]
    public partial class TestSceneTag : TagStorageTestScene
    {
        // Add visual tests to ensure correct behaviour of your game: https://github.com/ppy/osu-framework/wiki/Development-and-Testing
        // You can make changes to classes associated with the tests and they will recompile and update immediately.

        public TestSceneTag()
        {
            Add(new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Children =
                [
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Gray,
                    },
                    new FillFlowContainer
                    {
                        Size = new Vector2(400),
                        Spacing = new Vector2(10),
                        Children =
                        [
                            new Tag(new TagEntity { Name = "Anime", Color = Colour4.Red.Darken(0.2f).ToHex() }),
                            new Tag(new TagEntity { Name = "Manga", Color = Colour4.Red.ToHex() }),
                            new Tag(new TagEntity { Name = "One Piece", Color = Colour4.Lime.Darken(0.2f).ToHex() }),
                            new Tag(new TagEntity { Name = "Steins;Gate", Color = Colour4.Aqua.Darken(0.2f).ToHex() }),
                            new Tag(new TagEntity { Name = "Mushoku Tensei", Color = Colour4.Purple.ToHex() }),
                        ]
                    }
                ]
            });
        }
    }
}
