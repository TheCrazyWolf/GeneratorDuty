using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.Extensions;

public static class TelegramExtensions
{
    public static async Task TrySendMessage(this ITelegramBotClient client, long chatId, string message, IReplyMarkup? replyMarkup = null)
    {
        try
        {
            await client.SendTextMessageAsync(chatId, message, replyMarkup: replyMarkup, parseMode: ParseMode.Html);
        }
        catch (Exception e)
        {
            Console.WriteLine();
        }
    }
    
    public static async Task TryDeleteMessage(this ITelegramBotClient client, long chatId, int messageId)
    {
        try
        {
            await client.DeleteMessageAsync(chatId, messageId);
        }
        catch (Exception e)
        {
            Console.WriteLine();
        }
    }
}