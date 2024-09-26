using ClientSamgk;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Utils;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace GeneratorDuty.BackgroundServices;

public class AutoSendSchedule(ITelegramBotClient client, DutyContext ef, ClientSamgkApi clientSamgkApi) : BaseTask
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
                await Task.Delay(10000);
                continue;
            }

            var g = ef.ScheduleProps
                .Where(x => x.IsAutoSend).ToList();
            
            var dateTime = DateTime.Now;

            // если время вечернее смотрим расписание на перед
            if (dateTime.Hour >= 10)
                dateTime = dateTime.AddDays(1);
            
            // если день выходной, то пропускаем и добавляем дни пока не попадется рабочий
            while (dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                dateTime = dateTime.AddDays(1);
            }
            
            foreach (var item in g)
            {
                var result = await clientSamgkApi.Schedule
                    .GetScheduleAsync(DateOnly.FromDateTime(dateTime), item.SearchType, item.Value);

                if (result.Lessons.Count is 0) continue;
                
                var newResult = result.GetStringFromRasp();

                if (item.LastResult == newResult)
                {
                    await Task.Delay(10000);
                    continue;
                }
                
                try
                {
                    await client.SendTextMessageAsync(item.IdPeer, newResult, parseMode:ParseMode.Html);
                    item.LastResult = newResult;
                    ef.Update(item);
                    await ef.SaveChangesAsync();
                }
                catch 
                {
                    await Task.Delay(10000);
                    // ignored
                }
            }
            
            // задержка 30 мин
            await Task.Delay(300000);
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