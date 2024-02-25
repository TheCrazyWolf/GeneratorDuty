using GeneratorDuty.Services;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace GeneratorDuty.Commands;

public class UpdateCommand
{
    public static async Task ExecuteCommand(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message?.Text != null && update.Type == UpdateType.Message )
        {
            try
            {
                var splited = update.Message.Text.Split(' ');

                var newArrayStudents = splited[1].Split(';');

                var json = JsonConvert.SerializeObject(newArrayStudents);

                File.WriteAllText("students.json", json);

                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Списки обновлены");
            }
            catch (Exception e)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"Ошибка: \n{e.Message}");
            }
        }
    }
}