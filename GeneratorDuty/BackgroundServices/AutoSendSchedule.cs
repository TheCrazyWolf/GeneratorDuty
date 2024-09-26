using System.Timers;
using ClientSamgk;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Utils;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Timer =  System.Timers.Timer;

namespace GeneratorDuty.BackgroundServices;

public class AutoSendSchedule(ITelegramBotClient client, DutyContext ef, ClientSamgkApi clientSamgkApi) : BaseTask
{
    private readonly Timer _timer = new Timer
    {
        Interval = 300000, // 300000
    };
    
    public override Task RunAsync()
    {
        _timer.Elapsed += OnEventExecution;
        _timer.Start();
        return Task.CompletedTask;
    }

    private async void OnEventExecution(object? sender, ElapsedEventArgs e)
    {
        var dateTime = DateTime.Now;
        
        if(!CanWorkSerivce(DateTime.Now)) return;
        
        var scheduleProps = ef.ScheduleProps.Where(x => x.IsAutoSend).ToList();
        
        // если время вечернее смотрим расписание на перед
        if (dateTime.Hour >= 10)
            dateTime = dateTime.AddDays(1);
            
        // если день выходной, то пропускаем и добавляем дни пока не попадется рабочий
        while (dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            dateTime = dateTime.AddDays(1);
        }
        
        foreach (var item in scheduleProps)
        {
            var result = await clientSamgkApi.Schedule
                .GetScheduleAsync(DateOnly.FromDateTime(dateTime), item.SearchType, item.Value);

            if (result.Lessons.Count is 0) continue;
                
            var newResult = result.GetStringFromRasp();

            if (item.LastResult == newResult) continue;
                
            try
            {
                await client.SendTextMessageAsync(item.IdPeer, newResult, parseMode: ParseMode.Html);
                item.LastResult = newResult;
                ef.Update(item);
                await ef.SaveChangesAsync();
            }
            catch { //
            }
        }
    }
    
    private bool CanWorkSerivce(DateTime nowTime)
    {
        return nowTime.Hour switch
        {
            >= 19 or <= 7 => false,
            _ => true
        };
    }
}