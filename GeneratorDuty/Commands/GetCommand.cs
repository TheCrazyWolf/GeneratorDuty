using GeneratorDuty.Services;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GeneratorDuty.Commands;

public class GetCommand
{
    public static async Task ExecuteCommand(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message?.Text != null && update.Type == UpdateType.Message &&
            CommandHelper.GetReplacedCommandFromDomain(update.Message?.Text) == "/get")
        {
            if (!System.IO.File.Exists("students.json"))
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Файл students.json не существует");
                return;
            }

            var json = System.IO.File.ReadAllText("students.json");
            var students = JsonConvert.DeserializeObject<IList<string>>(json);

            if (students is null)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Ошибка чтения списка");
                return;
            }

            try
            {
                Random rnd = new Random();
                int rndInd = rnd.Next(0, students.Count);
                string duntyStudent = students[rndInd];

                await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"Дежурный: {duntyStudent}");
            }
            catch (Exception e)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"Ошибка: \n{e.Message}");
            }
        }
    }
}