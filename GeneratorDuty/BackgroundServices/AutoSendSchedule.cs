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
                await Task.Delay(GetDelayFromDateTime(DateTime.Now));
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
                    await Task.Delay(GetDelayFromDateTime(DateTime.Now));
                    continue;
                }
                
                try
                {
                    await client.SendTextMessageAsync(item.IdPeer, newResult, parseMode:ParseMode.Html);
                    item.LastResult = newResult;
                    ef.Update(item);
                    await ef.SaveChangesAsync();
                    continue;
                }
                catch 
                {
                    await Task.Delay(GetDelayFromDateTime(DateTime.Now));
                    // ignored
                }
                
                await Task.Delay(GetDelayFromDateTime(DateTime.Now));
            }
            
            // задержка 30 мин
            await Task.Delay(GetDelayFromDateTime(DateTime.Now));
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

    private int GetDelayFromDateTime(DateTime dateTime)
    {
        switch (dateTime.Hour)
        {
            case >= 10 and <= 16:
                return 300000; // 5 мин
            case >= 17 and <= 19:
                return 900000; // 15 минут
            default:
                return 1800000; // 30 минут
        }
    }
}