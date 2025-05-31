using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using TagStorage.App.Selector;

namespace TagStorage.App.Tests.Visual
{
    [TestFixture]
    public partial class TestSceneSelector : TagStorageTestScene
    {
        public TestSceneSelector()
        {
            Add(new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(400),
                Children =
                [
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Gray,
                    },
                    new SelectorContainer<TestSelectItem, string>()
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new[]
                        {
                            new TestSelectItem(),
                            new TestSelectItem(),
                            new TestSelectItem(),
                            new TestSelectItem(),
                            new TestSelectItem(),
                        }
                    }
                ]
            });
        }
    }
}
