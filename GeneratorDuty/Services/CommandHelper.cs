namespace GeneratorDuty.Services;

public class CommandHelper
{
    private static string _botDomain = "@generatorsgk_bot";

    public static string GetReplacedCommandFromDomain(string messageText)
        => messageText.Replace(_botDomain, string.Empty);
}