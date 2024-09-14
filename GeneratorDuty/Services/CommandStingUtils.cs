namespace GeneratorDuty.Services;

public static class CommandStingUtils
{
    public static string Me = "";
    private static string _botDomain = $"@{Me}";
    public static string GetReplacedCommandFromDomain(this string messageText)
        => messageText.Replace(_botDomain, string.Empty);
}