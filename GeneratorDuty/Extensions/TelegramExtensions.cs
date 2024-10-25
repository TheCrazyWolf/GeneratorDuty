using Telegram.Bot;
using Telegram.Bot.Types;
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
    
    public static async Task TryEditMessage(this ITelegramBotClient client,
        long chatId, int messageIdm, string message, IReplyMarkup? replyMarkup = null)
    {
        try
        {
            await client.EditMessageTextAsync(chatId: chatId, messageId: messageIdm, message,
                replyMarkup: replyMarkup as InlineKeyboardMarkup,
                parseMode: ParseMode.Html);
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
    
    public static async Task TrySendDocument(this ITelegramBotClient client, long chatId, InputFile file)
    {
        try
        {
            await client.SendDocumentAsync(chatId, file);
        }
        catch (Exception e)
        {
            Console.WriteLine();
        }
    }
    
    public static async Task<bool> IsUserAdminInChat(this ITelegramBotClient botClient, long userId, long chatId)
    {
        try
        {
            var chatMember = await botClient.GetChatAdministratorsAsync(chatId: chatId);
            return chatMember.Any(x => x.User.Id == userId);
        }
        catch
        {
            return false;
        }
    }
}