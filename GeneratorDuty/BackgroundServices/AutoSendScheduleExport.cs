using System.Timers;
using ClientSamgk;
using GeneratorDuty.BuilderHtml;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using Telegram.Bot;
using Telegram.Bot.Types;
using Timer =  System.Timers.Timer;

namespace GeneratorDuty.BackgroundServices;

public class AutoSendScheduleExport(ITelegramBotClient client, DutyContext ef, ClientSamgkApi clientSamgkApi) : BaseTask
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
        
        var scheduleProps = ef.ScheduleProps.Where(x => x.IsAutoExport).ToList();
        
        if(dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) return;

        foreach (var item in scheduleProps.Where(item => item.LastResult
                                                         != DateTime.Now.ToString("yyyy-MM-dd")))
        {
            try
            {
                var builderSchedule = new HtmlBuilderSchedule();

                var allExportResult = await clientSamgkApi.Schedule
                    .GetAllScheduleAsync(DateOnly.FromDateTime(dateTime), item.SearchType, 1500);

                if (allExportResult.Count is 0) continue;

                foreach (var scheduleFromDate in allExportResult)
                    builderSchedule.AddRow(scheduleFromDate, item.SearchType);

                await client.SendDocumentAsync(item.IdPeer,
                    new InputFileStream(builderSchedule.GetStreamFile(),
                        $"{DateOnly.FromDateTime(dateTime)}_{item.SearchType}.html"));

                item.LastResult = DateTime.Now.ToString("yyyy-MM-dd");
                ef.Update(item);
                await ef.SaveChangesAsync();
            }
            catch
            {
                //
            }
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
}