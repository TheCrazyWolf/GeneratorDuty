using System.Timers;
using ClientSamgk;
using GeneratorDuty.BuilderHtml;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;
using Timer =  System.Timers.Timer;

namespace GeneratorDuty.BackgroundServices;

public class AutoSendScheduleExport(ITelegramBotClient client, DutyRepository repository, 
    ClientSamgkApi clientSamgkApi) : BackgroundService
{
    private readonly Timer _timer = new Timer
    {
        Interval = 300000, // 300000
    };

    private async void OnEventExecution(object? sender, ElapsedEventArgs e)
    {
        var dateTime = DateTime.Now;
        
        if(!CanWorkSerivce(DateTime.Now)) return;

        var scheduleProps = await repository.ScheduleProps.GetSchedulePropsFromAutoExport(true);
        
        if(dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) return;

        foreach (var item in scheduleProps.Where(item => item.LastResult
                                                         != DateTime.Now.ToString("yyyy-MM-dd")))
        {
            var builderSchedule = new HtmlBuilderSchedule();

            var allExportResult = await clientSamgkApi.Schedule
                .GetAllScheduleAsync(DateOnly.FromDateTime(dateTime), item.SearchType, 1500);

            if (allExportResult.Count is 0) continue;

            foreach (var scheduleFromDate in allExportResult)
                builderSchedule.AddRow(scheduleFromDate, item.SearchType);

            await client.TrySendDocument(item.IdPeer,
                new InputFileStream(builderSchedule.GetStreamFile(),
                    $"{DateOnly.FromDateTime(dateTime)}_{item.SearchType}.html"));

            item.LastResult = DateTime.Now.ToString("yyyy-MM-dd");
            await repository.ScheduleProps.Update(item);
            await Task.Delay(1000);
        }
    }
    
    private bool CanWorkSerivce(DateTime nowTime)
    {
        return nowTime.Hour switch
        {
            >= 12 or <= 7 => false,
            _ => true
        };
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer.Elapsed += OnEventExecution;
        _timer.Start();
        return Task.CompletedTask;
    }
}