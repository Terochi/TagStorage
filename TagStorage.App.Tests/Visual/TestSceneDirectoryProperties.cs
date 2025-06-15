using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using TagStorage.App.DirectoryBrowser;

namespace TagStorage.App.Tests.Visual;

[TestFixture]
public partial class TestSceneDirectoryProperties : TagStorageTestScene
{
    public TestSceneDirectoryProperties()
    {
        var directoryProperties = new DirectoryProperties
        {
            Height = 20,
            RelativeSizeAxes = Axes.X,
        };
        directoryProperties.Properties.Add(new DirectoryPropertyValue(DirectoryPropertyType.Name));
        directoryProperties.Properties.Add(new DirectoryPropertyValue(DirectoryPropertyType.Tags));
        directoryProperties.Properties.Add(new DirectoryPropertyValue(DirectoryPropertyType.Size));
        Add(directoryProperties);

        AddStep("Add DateModified", () =>
        {
            directoryProperties.Properties.Add(new DirectoryPropertyValue(DirectoryPropertyType.DateModified));
        });
        AddStep("Remove DateModified", () =>
        {
            directoryProperties.Properties.RemoveAll(p => p.Type == DirectoryPropertyType.DateModified);
        });

        directoryProperties.Properties.BindCollectionChanged((_, _) =>
        {
            Logger.Log(string.Join(", ", directoryProperties.Properties.Select(v => $"{v.Type}: {v.Width}")));
        });
    }
}
