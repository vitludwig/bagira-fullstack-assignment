using System.Text.RegularExpressions;

namespace Backend.Infrastructure.Utilities;

public static partial class GridifyFilterBuilder
{
    public static string Contains(string field, string value)
    {
        return $"{field}=*{Escape(value)}/i";
    }

    public static string Equals<T>(string field, T value)
    {
        return $"{field}={Escape(value?.ToString() ?? string.Empty)}";
    }

    public static string And(IEnumerable<string> conditions)
    {
        return string.Join(",", conditions);
    }

    public static string Or(params string[] conditions)
    {
        return $"({string.Join("|", conditions)})";
    }

    private static string Escape(string value)
    {
        return SpecialCharacters().Replace(value, @"\$1");
    }

    [GeneratedRegex(@"([(),|\\]|/i)")]
    private static partial Regex SpecialCharacters();
}
