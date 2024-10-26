using System.Timers;
using ClientSamgk;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using GeneratorDuty.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Timer =  System.Timers.Timer;

namespace GeneratorDuty.BackgroundServices;

public class AutoSendSchedule(ITelegramBotClient client, DutyRepository repository, 
    ClientSamgkApi clientSamgkApi, ILogger<AutoSendSchedule> logger) : BackgroundService
{
    private readonly Timer _timer = new Timer
    {
        Interval = 300000, // 300000
    };

    private async void OnEventExecution(object? sender, ElapsedEventArgs e)
    {
        var dateTime = DateTime.Now;
        
        if(!CanWorkSerivce(DateTime.Now)) return;
        
        logger.LogInformation($"Запуск скрипта по расписанию");
        
        var scheduleProps = await repository.ScheduleProps.GetSchedulePropsFromAutoSend(true);
        
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
            logger.LogInformation($"Скрипт № {item.Id} начал работать");
            
            var result = await clientSamgkApi.Schedule
                .GetScheduleAsync(DateOnly.FromDateTime(dateTime), item.SearchType, item.Value);

            if (result.Lessons.Count is 0) continue;
            
            if (item.LastResult == result.GetMd5()) continue;
                
            var success = await client.TrySendMessage(item.IdPeer, result.GetStringFromRasp());

            if (!success) item.Fails++;
            
            item.LastResult = result.GetMd5();
            await repository.ScheduleProps.Update(item);
            logger.LogInformation($"Скрипт № {item.Id} отработан");

            if (item.Fails >= 10) await repository.ScheduleProps.Remove(item);
            
            await Task.Delay(1000);
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

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer.Elapsed += OnEventExecution;
        _timer.Start();
        logger.LogInformation($"Запущен сервис");
        return Task.CompletedTask;
    }
}