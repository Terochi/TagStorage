using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using SHA = System.Security.Cryptography.SHA1;

namespace TagStorage.Library.Helper;

public static class DirectoryUtils
{
    private const long max_file_size_to_hash = 1024 * 1024 * 256;
    private const int max_subfolders_to_hash = 256;
    private static int subfolderCounter;

    private static string shortFormatDate(DateTime dateTime) =>
        dateTime.ToUniversalTime().ToString("yyMMddHHmmss", CultureInfo.InvariantCulture);

    public static (string? hash, long size) CreateHash(DirectoryInfo directory)
    {
        StringBuilder footprint = new StringBuilder();
        long totalSize = 0;
        subfolderCounter = 0;
        addToFootprint(footprint, directory, ref totalSize);
        footprint.Append('=');
        footprint.Append(totalSize);
        string hash = Convert.ToBase64String(SHA.HashData(Encoding.UTF8.GetBytes(footprint.ToString())), Base64FormattingOptions.None);
        return (hash, totalSize);
    }

    public static (string? hash, long size) CreateHash(FileInfo file)
    {
        using FileStream fs = File.OpenRead(file.FullName);
        long totalSize = fs.Length;
        if (totalSize > max_file_size_to_hash)
            return (null, totalSize);

        string hash = Convert.ToBase64String(SHA.HashData(fs), Base64FormattingOptions.None);
        return (hash, totalSize);
    }

    private static void addToFootprint(StringBuilder footprint, DirectoryInfo parent, ref long totalSize, int searchedPathLength = 0, [CanBeNull] Ignore.Ignore ignore = null)
    {
        if (++subfolderCounter >= max_subfolders_to_hash)
            return;

        IEnumerable<FileInfo> files = parent.GetFiles().AsEnumerable();
        IEnumerable<DirectoryInfo> directories = parent.GetDirectories().AsEnumerable();

        FileInfo? gitignore = files.FirstOrDefault(file => file.Name.Equals(".gitignore", StringComparison.OrdinalIgnoreCase));

        if (gitignore != null)
        {
            ignore = new Ignore.Ignore();
            searchedPathLength = parent.FullName.Length + 1;

            foreach (string rule in File.ReadLines(gitignore.FullName))
            {
                try
                {
                    ignore.Add(rule);
                }
                catch { }
            }

            footprint.Append(shortFormatDate(gitignore.LastWriteTime));
        }

        if (ignore != null)
        {
            files = files.Where(file => !ignore.IsIgnored(file.FullName.Substring(searchedPathLength)));
            directories = directories.Where(dir => !ignore.IsIgnored(dir.FullName.Substring(searchedPathLength)));
        }

        foreach (FileInfo file in files.OrderBy(file => file.Name, StringComparer.InvariantCulture))
        {
            footprint.Append(shortFormatDate(file.LastWriteTime));
            footprint.Append(file.Name);
            totalSize += file.Length;
        }

        foreach (DirectoryInfo directory in directories.OrderBy(dir => dir.Name, StringComparer.InvariantCulture))
        {
            if (directory.Name == ".git") continue;

            footprint.Append('/');
            footprint.Append(directory.Name);
            footprint.Append('>');
            addToFootprint(footprint, directory, ref totalSize, searchedPathLength, ignore);
            footprint.Append('<');
        }
    }
}
