using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using GeneratorDuty.Repository.Duty;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands.Service;

public class GetIdCommand() : BaseCommand
{
    public override string Command { get; } = "/id";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        string msg = $"ChatID: {message.Chat.Id}\nID: {message.From.Id}";
        await client.TrySendMessage(message.Chat.Id, msg);
    }
}