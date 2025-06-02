using System.IO.Compression;
using System.Text;

namespace TagStorage.Library.Helper;

public static class StringUtils
{
    public static bool FuzzyMatch(this string text, string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return false;

        int patternIndex = 0;
        int textIndex = 0;

        while (patternIndex < pattern.Length && textIndex < text.Length)
        {
            if (char.ToLower(pattern[patternIndex]) == char.ToLower(text[textIndex]))
            {
                patternIndex++;
            }

            textIndex++;
        }

        return patternIndex == pattern.Length;
    }

    public static string Compress(string s)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(s);

        using (var output = new MemoryStream())
        {
            using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(bytes, 0, bytes.Length);
            }

            return Convert.ToBase64String(output.ToArray());
        }
    }

    public static string Decompress(string s)
    {
        byte[] compressed = Convert.FromBase64String(s);

        using (var input = new MemoryStream(compressed))
        using (var gzip = new GZipStream(input, CompressionMode.Decompress))
        using (var output = new MemoryStream())
        {
            gzip.CopyTo(output);
            return Encoding.UTF8.GetString(output.ToArray());
        }
    }
}
