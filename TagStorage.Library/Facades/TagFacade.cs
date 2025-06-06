using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using TagStorage.Library.Entities;
using TagStorage.Library.Helper;
using TagStorage.Library.Repositories;

namespace TagStorage.Library.Facades;

public partial class TagFacade : IFacadeBase
{
    [Resolved]
    private TagRepository tags { get; set; }

    [Resolved]
    private ChangeRepository changes { get; set; }

    [Resolved]
    private FileRepository files { get; set; }

    [Resolved]
    private FileLocationRepository fileLocations { get; set; }

    [Resolved]
    private FileTagRepository fileTags { get; set; }

    public IEnumerable<TagEntity> Get()
    {
        return tags.Get();
    }

    public IEnumerable<TagEntity> Get(string name)
    {
        return Get().Where(t => t.Name.FuzzyMatch(name));
    }

    public TagEntity Insert(string tagName)
    {
        Colour4 newColor = Colour4.FromHSL(Random.Shared.NextSingle(), Random.Shared.NextSingle() * 0.56f + 0.42f, Random.Shared.NextSingle() * 0.35f + 0.4f);
        return tags.Insert(new TagEntity { Color = newColor.ToHex(), Name = tagName });
    }

    public void Delete(TagEntity tag)
    {
        tags.Delete(tag);
    }

    public IEnumerable<FileLocationEntity> GetFileSelectedTags(IEnumerable<TagEntity> selectedTags)
    {
        IEnumerable<int> selectedTagIds = selectedTags.Select(t => t.Id);
        IEnumerable<int> taggedFileIds =
            selectedTags.SelectMany(tag => fileTags.GetByTag(tag.Id))
                        .GroupBy(file => file.File)
                        .Where(g => selectedTagIds.All(tag => g.Any(f => f.Tag == tag)))
                        .Select(g => g.Key);

        return taggedFileIds.Select(fileLocations.Get).Where(loc => loc != null)!;
    }

    public void TagFile(string tagName, string fullname)
    {
        TagEntity tag = Get(tagName).FirstOrDefault() ?? Insert(tagName);

        TagFile(tag, fullname);
    }

    public void TagFile(TagEntity tag, string fullname)
    {
        string machineName = Environment.MachineName;

        string? hash;
        long size;
        FileLocationType type;
        DateTime lastModified;

        if (Directory.Exists(fullname))
        {
            var directoryInfo = new DirectoryInfo(fullname);
            lastModified = DateTime.UtcNow;
            (hash, size) = DirectoryUtils.CreateHash(directoryInfo);
            type = FileLocationType.D;
        }
        else
        {
            var fileInfo = new FileInfo(fullname);
            lastModified = fileInfo.LastWriteTimeUtc;
            (hash, size) = DirectoryUtils.CreateHash(fileInfo);
            type = FileLocationType.F;
        }

        FileEntity file;
        FileLocationEntity? location = fileLocations.GetByPath(fullname).FirstOrDefault(l => l.Machine == machineName);

        if (location == null)
        {
            file = files.Insert(new FileEntity());
            location = fileLocations.Insert(new FileLocationEntity
            {
                File = file.Id,
                Machine = machineName,
                Path = fullname,
                Type = type,
            });
        }
        else if (location.Type != type)
        {
            Logger.Log($"File type does not match with existing {location.Path}", level: LogLevel.Error);
            return;
        }
        else
        {
            file = files.Get(location.File)!;
        }

        IEnumerable<ChangeEntity> foundChanges = changes.FindDuplicates(new ChangeEntity
        {
            Hash = hash,
            Size = size
        });

        bool foundIdentical = false;

        if (foundChanges.Any())
        {
            Logger.Log($"Found duplicate changes for {location.Path}");

            foreach (ChangeEntity change in foundChanges)
            {
                FileLocationEntity loc = fileLocations.Get(change.Location)!;
                Logger.Log($"Duplicate file: {loc.Path}");
                foundIdentical |= change.Location == location.Id;
            }

            // TODO: More handling for matching with already existing FileEntity...
        }

        if (!foundIdentical)
        {
            changes.Insert(new ChangeEntity
            {
                Hash = hash,
                Size = size,
                Date = lastModified,
                Location = location.Id,
            });
        }

        FileTagEntity fileTag = new FileTagEntity
        {
            File = file.Id,
            Tag = tag.Id
        };

        if (!fileTags.Exists(fileTag))
        {
            fileTags.Insert(fileTag);
        }
    }
}
