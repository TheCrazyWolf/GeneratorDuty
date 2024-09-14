using System.Text;
using ClientSamgk;
using ClientSamgkOutputResponse.Interfaces.Schedule;
using GeneratorDuty.Database;
using GeneratorDuty.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands;

public class TomorrowCommand(DutyContext ef) : TodayCommand(ef)
{
    private readonly ClientSamgkApi _clientSamgk = new ClientSamgkApi();
    private readonly DutyContext _ef = ef;
    public override string Command { get; } = "/tomorrow";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        message.Text = message.Text.GetReplacedCommandFromDomain().Replace(Command, string.Empty);

        var prop = await _ef.ScheduleProps.FirstOrDefaultAsync(x=> x.IdPeer == message.From.Id);
        
        if(prop is null)
        {
            await client.SendTextMessageAsync(message.From.Id, $"Не смог найти настройки для Вашей беседы, задай /set <группа, фио препода, кабинет>");
            return;
        }
        
        var currentDateTime = DateTime.Now;


        while (currentDateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            currentDateTime = currentDateTime.AddDays(1);
        }
        
        var result = await _clientSamgk.Sсhedule.GetScheduleAsync(DateOnly.FromDateTime(currentDateTime), 
            prop.SearchType, prop.Value);
        
        await client.SendTextMessageAsync(message.From.Id, GetStringFromRasp(result));
    }
}