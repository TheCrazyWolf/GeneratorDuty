using ClientSamgk;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using GeneratorDuty.Utils;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GeneratorDuty.Commands;

public class TodayCommand(DutyRepository repository, ClientSamgkApi clientSamgk) : BaseCommand
{
    public override string Command { get; } = "/today";

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
        
        var result = await clientSamgk.Schedule.GetScheduleAsync(DateOnly.FromDateTime(DateTime.Now), 
            prop.SearchType, prop.Value);
        
        await client.TrySendMessage(message.Chat.Id, result.GetStringFromRasp());
    }
}