using ClientSamgk;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using GeneratorDuty.Repository.Duty;
using GeneratorDuty.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.Commands.Schedule;

public class WidgetCommand(DutyRepository repository, ClientSamgkApi clientSamgk) : BaseCommand
{
    public override string Command { get; } = "/widget";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        message.Text = message.Text.GetReplacedCommandFromDomain().Replace(Command, string.Empty);
        
        var prop = await repository.ScheduleProps.GetSchedulePropFromChat(message.Chat.Id);
        
        if(prop is null)
        {
            await client.TrySendMessage(message.Chat.Id, $"ℹ️ Не смог найти настройки для Вашей беседы, задай /set <группа, фио препода, кабинет>");
            return;
        }

        var scheduleProp = await client.TrySendMessage(prop.IdPeer, "эт сообщение будет автоматически обновлено");
        
        
        
    }
}