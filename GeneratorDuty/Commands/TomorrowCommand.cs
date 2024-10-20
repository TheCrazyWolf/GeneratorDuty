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

public class TomorrowCommand(DutyRepository ef, ClientSamgkApi clientSamgk) : BaseCommand
{
    public override string Command { get; } = "/tomorrow";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        message.Text = message.Text.GetReplacedCommandFromDomain().Replace(Command, string.Empty);

        var prop = await ef.ScheduleProps.GetSchedulePropFromChat(message.Chat.Id);
        
        if(prop is null)
        {
            await client.TrySendMessage(message.Chat.Id, $"ℹ️ Не смог найти настройки для Вашей беседы, задай /set <группа, фио препода, кабинет>");
            return;
        }
        
        var currentDateTime = DateTime.Now.AddDays(1);
        
        while (currentDateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            currentDateTime = currentDateTime.AddDays(1);
        }
        
        var result = await clientSamgk.Schedule.GetScheduleAsync(DateOnly.FromDateTime(currentDateTime), 
            prop.SearchType, prop.Value);
        
        await client.TrySendMessage(message.Chat.Id, result.GetStringFromRasp());
    }
}