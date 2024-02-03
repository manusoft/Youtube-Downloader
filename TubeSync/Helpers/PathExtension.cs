namespace TubeSync.Helpers;

public static class PathExtension
{
    public static string ConvertToValidFileName(string inputString)
    {
        if (string.IsNullOrWhiteSpace(inputString))
        {
            throw new ArgumentException("Input string cannot be null or empty.", nameof(inputString));
        }

        // Replace or remove characters that are not allowed in file names
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string invalidCharsPattern = $"[{Regex.Escape(new string(invalidChars))}]";
        string validFileName = Regex.Replace(inputString, invalidCharsPattern, "");

        // Remove leading and trailing spaces
        validFileName = validFileName.Trim();

        // Ensure the file name is not empty after replacement
        if (string.IsNullOrWhiteSpace(validFileName))
        {
            throw new InvalidOperationException("The resulting file name is empty after conversion. " +
                "Ensure the input string contains valid characters.");
        }

        return validFileName;
    }
}
