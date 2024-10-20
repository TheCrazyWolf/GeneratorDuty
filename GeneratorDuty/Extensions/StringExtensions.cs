namespace GeneratorDuty.Extensions;

public static class StringExtensions
{
    public static string Me = "";
    private static string _botDomain = $"@{Me}";
    public static string GetReplacedCommandFromDomain(this string messageText)
        => messageText.Replace(_botDomain, string.Empty);
}