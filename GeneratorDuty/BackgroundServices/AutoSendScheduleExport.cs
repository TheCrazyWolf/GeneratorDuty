using System.Timers;
using ClientSamgk;
using GeneratorDuty.BuilderHtml;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Timer =  System.Timers.Timer;

namespace GeneratorDuty.BackgroundServices;

public class AutoSendScheduleExport(ITelegramBotClient client, DutyRepository repository, 
    ClientSamgkApi clientSamgkApi, ILogger<AutoSendScheduleExport> logger) : BackgroundService
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

        var scheduleProps = await repository.ScheduleProps.GetSchedulePropsFromAutoExport(true);
        
        if(dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) return;

        foreach (var item in scheduleProps.Where(item => item.LastResult != DateTime.Now.ToString("yyyy-MM-dd")))
        {
            logger.LogInformation($"Скрипт № {item.Id} начал работать");
            var builderSchedule = new HtmlBuilderSchedule();

            var allExportResult = await clientSamgkApi.Schedule
                .GetAllScheduleAsync(DateOnly.FromDateTime(dateTime), item.SearchType, 1500);

            if (allExportResult.Count is 0) continue;

            foreach (var scheduleFromDate in allExportResult)
                builderSchedule.AddRow(scheduleFromDate, item.SearchType);

            var success = await client.TrySendDocument(item.IdPeer,
                new InputFileStream(builderSchedule.GetStreamFile(),
                    $"{DateOnly.FromDateTime(dateTime)}_{item.SearchType}.html"));
            
            if (!success) item.Fails++;
            
            if (success)
            {
                item.Fails = 0;
                item.LastResult = DateTime.Now.ToString("yyyy-MM-dd");
                logger.LogInformation($"Скрипт № {item.Id} отработан");
            }
            
            await repository.ScheduleProps.Update(item);
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