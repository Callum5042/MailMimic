namespace MailMimic.Extensions;

/// <summary>
/// Extension methods for strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Splits a key value pair string into a tuple.
    /// </summary>
    /// <param name="input">Input string must contain a ':' to be split on.</param>
    /// <returns>A tuple containing the key and value of the string</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static (string key, string value) SplitKeyValue(this string input)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));

        if (!input.Contains(':'))
        {
            throw new InvalidOperationException("Invalid keyvalue pair");
        }

        var parts = input.Split(":", 2, StringSplitOptions.TrimEntries);
        return parts.Length == 2 ? (parts[0], parts[1]) : (parts[0], string.Empty);
    }
}
