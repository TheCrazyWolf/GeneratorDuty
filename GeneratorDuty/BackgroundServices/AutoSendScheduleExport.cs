using ClientSamgk;
using GeneratorDuty.BuilderHtml;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GeneratorDuty.BackgroundServices;

public class AutoSendScheduleExport(ITelegramBotClient client, DutyContext ef, ClientSamgkApi clientSamgkApi) : BaseTask
{
    public override Task RunAsync()
    {
        Task.Run(WorkSerivce);
        return Task.CompletedTask;
    }

    private async Task WorkSerivce()
    {
        while (true)
        {
            if(!CanWorkSerivce(DateTime.Now))
            {
                await Task.Delay(5000);
                continue;
            }

            var g = ef.ScheduleProps
                .Where(x => x.IsAutoExport).ToList();
            
            var dateTime = DateTime.Now;
            
            // если день выходной, то пропускаем и добавляем дни пока не попадется рабочий
            
            if(dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                await Task.Delay(5000);
                continue;
            }
            
            foreach (var item in g)
            {
                
                if (item.LastResult == DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    await Task.Delay(5000);
                    continue;
                }
                
                try
                {
                    var builderSchedule = new HtmlBuilderSchedule();
                    
                    var allExportResult = await clientSamgkApi.Schedule
                        .GetAllScheduleAsync(DateOnly.FromDateTime(dateTime), item.SearchType, 1500);

                    if(allExportResult.Count is 0)
                    {
                        await Task.Delay(5000);
                        continue;
                    }
                    
                    foreach (var scheduleFromDate in allExportResult)
                        builderSchedule.AddRow(scheduleFromDate, item.SearchType);

                    await client.SendDocumentAsync(item.IdPeer,
                        new InputFileStream(builderSchedule.GetStreamFile(), $"{DateOnly.FromDateTime(dateTime)}_{item.SearchType}.html"));
                    
                    item.LastResult = DateTime.Now.ToString("yyyy-MM-dd");
                    ef.Update(item);
                    await ef.SaveChangesAsync();
                }
                catch 
                {
                    await Task.Delay(5000);
                }
                
                await Task.Delay(2000);
            }
            
            // задержка 30 мин
            await Task.Delay(1800000);
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