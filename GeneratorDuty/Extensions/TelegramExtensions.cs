using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.Extensions;

public static class TelegramExtensions
{
    public static async Task<Message?> TrySendMessage(this ITelegramBotClient client, long chatId, string message, IReplyMarkup? replyMarkup = null)
    {
        try
        {
            return await client.SendTextMessageAsync(chatId, message, replyMarkup: replyMarkup, parseMode: ParseMode.Html);
        }
        catch 
        {
            return null;
        }
    }
    
    public static async Task<bool> TryPingMessage(this ITelegramBotClient client, long chatId, int messageId)
    {
        try
        {
            await client.PinChatMessageAsync(chatId, messageId);
            return true;
        }
        catch 
        {
            return false;
        }
    }
    
    public static async Task<bool> TryUnPingMessage(this ITelegramBotClient client, long chatId, int messageId)
    {
        try
        {
            await client.UnpinChatMessageAsync(chatId, messageId);
            return true;
        }
        catch 
        {
            return false;
        }
    }
    
    public static async Task<bool> TryEditMessage(this ITelegramBotClient client,
        long chatId, int messageIdm, string message, IReplyMarkup? replyMarkup = null)
    {
        try
        {
            await client.EditMessageTextAsync(chatId: chatId, messageId: messageIdm, message,
                replyMarkup: replyMarkup as InlineKeyboardMarkup,
                parseMode: ParseMode.Html);
            return true;
        }
        catch 
        {
            return false;
        }
    }
    
    public static async Task<bool> TryDeleteMessage(this ITelegramBotClient client, long chatId, int messageId)
    {
        try
        {
            await client.DeleteMessageAsync(chatId, messageId);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public static async Task<bool> TrySendDocument(this ITelegramBotClient client, long chatId, InputFile file)
    {
        try
        {
            await client.SendDocumentAsync(chatId, file);
            return true;
        }
        catch 
        {
            return false;
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