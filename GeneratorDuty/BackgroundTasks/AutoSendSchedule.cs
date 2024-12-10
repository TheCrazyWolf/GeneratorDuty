using System.Globalization;
using ClientSamgk;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository.Duty;
using GeneratorDuty.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace GeneratorDuty.BackgroundTasks;

public class AutoSendSchedule(
    ITelegramBotClient client,
    DutyRepository repository,
    ClientSamgkApi clientSamgkApi,
    ILogger<AutoSendSchedule> logger) : BackgroundServiceBase
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"Запущен сервис");
        
        while (true)
        {
            var dateTime = DateTime.Now;

            if (!CanWorkSerivce(DateTime.Now))
            {
                await Task.Delay(1000);
                continue;
            }

            logger.LogInformation($"Запуск скрипта по расписанию");

            var scheduleProps = await repository.ScheduleProps.GetSchedulePropsFromAutoSend(true);

            // если время вечернее смотрим расписание на перед
            if (dateTime.Hour >= 10) dateTime = dateTime.AddDays(1);

            // если день выходной, то пропускаем и добавляем дни пока не попадется рабочий
            while (dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) dateTime = dateTime.AddDays(1);

            var rules = await repository.ScheduleRules.GetRuleFromDateOrDefault(DateOnly.FromDateTime(dateTime));
            
            foreach (var item in scheduleProps)
            {
                logger.LogInformation($"Скрипт № {item.Id} начал работать");

                var result = await clientSamgkApi.Schedule
                    .GetScheduleAsync(DateOnly.FromDateTime(dateTime), item.SearchType, item.Value, rules.CallType, rules.ShowImportantLesson, rules.ShowRussianHorizont);

                if (result.Lessons.Count is 0)
                {
                    logger.LogInformation(
                        $"Скрипт № {item.Id} отработан: Расписание на {dateTime.ToString(CultureInfo.InvariantCulture)} - 0 пар");
                    await Task.Delay(1000);
                    continue;
                }

                var md5New = result.GetMd5();

                if (item.LastResult == md5New)
                {
                    logger.LogInformation(
                        $"Скрипт № {item.Id} отработан: Прошлый результат: {item.LastResult}. Новый {md5New}");
                    await Task.Delay(1000);
                    continue;
                }

                var success = await client.TrySendMessage(item.IdPeer, result.GetStringFromRasp());

                if (success is null)
                {
                    item.Fails++;
                    logger.LogInformation($"Скрипт № {item.Id} не отработан: Ошибки при отправке сообщения");
                }

                if (success is not null)
                {
                    item.Fails = 0;
                    item.LastResult = md5New;
                    logger.LogInformation($"Скрипт № {item.Id} отработан: ОК");
                }

                await repository.ScheduleProps.Update(item);
                /*if (item.Fails >= 10)
                {
                    logger.LogInformation($"Скрипт № {item.Id} удалена из за большого кол-во ошибок");
                    await repository.ScheduleProps.Remove(item);
                }*/

                logger.LogInformation($"Скрипт № {item.Id} Завершен");
                await Task.Delay(1500);
            }
            
            await Task.Delay(300000);
        }
    }
}