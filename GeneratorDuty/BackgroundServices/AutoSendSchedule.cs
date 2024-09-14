using ClientSamgk;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Utils;
using Telegram.Bot;

namespace GeneratorDuty.BackgroundServices;

public class AutoSendSchedule(ITelegramBotClient client, DutyContext ef) : BaseTask
{
    private readonly ClientSamgkApi _clientSamgkApi = new ClientSamgkApi();
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
                .Where(x => x.IsAutoSend).ToList();
            
            var dateTime = DateTime.Now;

            // если время вечернее смотрим расписание на перед
            if (dateTime.Hour >= 14)
                dateTime = dateTime.AddDays(1);
            
            // если день выходной, то пропускаем и добавляем дни пока не попадется рабочий
            while (dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                dateTime = dateTime.AddDays(1);
            }
            
            foreach (var item in g)
            {
                var result = await _clientSamgkApi.Schedule
                    .GetScheduleAsync(DateOnly.FromDateTime(dateTime), item.SearchType, item.Value);

                if (result.Lessons.Count is 0) continue;
                
                var newResult = result.GetStringFromRasp();
                
                if(item.LastResult == newResult) continue;
                
                try
                {
                    await client.SendTextMessageAsync(item.IdPeer, newResult);
                    item.LastResult = newResult;
                    ef.Update(item);
                    await ef.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    // ignored
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
            >= 19 or <= 7 => false,
            _ => true
        };
    }
}