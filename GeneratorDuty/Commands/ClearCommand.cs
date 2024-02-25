using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = Telegram.Bot.Types.File;

namespace GeneratorDuty.Commands;

public class ClearCommand
{
    public static async Task ExecuteCommand(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message?.Text != null && update.Type == UpdateType.Message )
        {
            System.IO.File.Delete("students.json");
            
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Файл students.json удален");
        }
    }
}