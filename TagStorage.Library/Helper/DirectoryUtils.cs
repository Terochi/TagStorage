using System.Globalization;
using System.Text;
using SHA = System.Security.Cryptography.SHA256;

namespace TagStorage.Library.Helper;

public static class DirectoryUtils
{
    private static string shortFormatDate(DateTime dateTime) =>
        dateTime.ToUniversalTime().ToString("yyMMddHHmmss", CultureInfo.InvariantCulture);

    public static string CreateHash(DirectoryInfo directory)
    {
        StringBuilder footprint = new StringBuilder();
        long totalSize = 0;
        addToFootprint(footprint, directory, ref totalSize);
        footprint.Append('=');
        footprint.Append(totalSize);
        return Convert.ToBase64String(SHA.HashData(Encoding.UTF8.GetBytes(footprint.ToString())), Base64FormattingOptions.None);
    }

    public static string CreateHash(FileInfo file)
    {
        return Convert.ToBase64String(SHA.HashData(File.ReadAllBytes(file.FullName)), Base64FormattingOptions.None);
    }

    private static void addToFootprint(StringBuilder footprint, DirectoryInfo parent, ref long totalSize, int searchedPathLength = 0, Ignore.Ignore? ignore = null)
    {
        IEnumerable<FileInfo> files = parent.GetFiles().AsEnumerable();
        IEnumerable<DirectoryInfo> directories = parent.GetDirectories().AsEnumerable();

        FileInfo? gitignore = files.FirstOrDefault(file => file.Name.Equals(".gitignore", StringComparison.OrdinalIgnoreCase));

        if (gitignore != null)
        {
            ignore = new Ignore.Ignore();
            searchedPathLength = parent.FullName.Length + 1;
            ignore.Add(File.ReadLines(gitignore.FullName));

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
