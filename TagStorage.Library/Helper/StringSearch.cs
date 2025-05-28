namespace TagStorage.Library.Helper;

public static class StringSearch
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
}
