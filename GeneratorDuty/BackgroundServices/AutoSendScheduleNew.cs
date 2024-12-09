using System.Globalization;
using ClientSamgk;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using GeneratorDuty.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace GeneratorDuty.BackgroundServices;

public class AutoSendScheduleNew(
    ITelegramBotClient client, DutyRepository repository, ClientSamgkApi clientSamgkApi,
    ILogger<AutoSendScheduleNew> logger) : BackgroundServiceBase
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"Запущен сервис");
        
        while (true)
        {
            if (!CanWorkSerivce(DateTime.Now))
            {
                await Task.Delay(1000);
                return;
            }

            logger.LogInformation($"Запуск скрипта по расписанию");

            var scheduleProps = await repository.ScheduleProps.GetSchedulePropsFromAutoSend(true, true);
            
            foreach (var scheduleProp in scheduleProps)
            {
                var date = DateOnly.FromDateTime(DateTime.Now);
                
                logger.LogInformation($"Скрипт № {scheduleProp.Id} начал работать");
                for (int i = 0; i <= 5; i++)
                {
                    // если день выходной, то пропускаем и добавляем дни пока не попадется рабочий
                    while (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) date = date.AddDays(1);

                    var rules = await repository.ScheduleRules.GetRuleFromDateOrDefault(date);
                    
                    logger.LogInformation($"Скрипт № {scheduleProp.Id}. Проверка: {date.ToString(CultureInfo.InvariantCulture)}");
                    var history = await repository.ScheduleHistory.CreateOrGetScheduleHistory(scheduleProp.IdPeer, date);
                    
                    var result = await clientSamgkApi.Schedule.GetScheduleAsync(date, scheduleProp.SearchType, scheduleProp.Value, rules.CallType, rules.ShowImportantLesson, rules.ShowRussianHorizont);
                    
                    if (result.Lessons.Count is 0)
                    {
                        logger.LogInformation($"Скрипт № {scheduleProp.Id} отработан: Расписание на {date.ToString()} - 0 пар");
                        await Task.Delay(1000);
                        date = date.AddDays(1);
                        continue; 
                    }
                    
                    if (history.Result == result.GetStringFromRasp())
                    {
                        logger.LogInformation($"Скрипт № {scheduleProp.Id} отработан: Расписание на {date.ToString()} - не изменилось");
                        await Task.Delay(1000);
                        date = date.AddDays(1);
                        continue;
                    }
                    
                    var success = await client.TrySendMessage(scheduleProp.IdPeer, result.GetStringFromRasp());

                    if (!success)
                    {
                        scheduleProp.Fails++;
                        logger.LogInformation($"Скрипт № {scheduleProp.Id} не отработан: Расписание на {date.ToString()}. Ошибки при отправке сообщения");
                    }

                    if (success)
                    {
                        scheduleProp.Fails = 0;
                        history.Result = result.GetStringFromRasp();
                        logger.LogInformation($"Скрипт № {scheduleProp.Id} отработан: Расписание на {date.ToString()}");
                    }

                    await repository.ScheduleProps.Update(scheduleProp);
                    await repository.ScheduleHistory.UpdateScheduleHistory(history);
                    date = date.AddDays(1);
                    await Task.Delay(3000);
                }
                
                await Task.Delay(3000);
            }
            
            await Task.Delay(300000);
        }
    }
}