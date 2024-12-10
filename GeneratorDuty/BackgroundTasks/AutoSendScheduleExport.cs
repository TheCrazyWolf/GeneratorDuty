using System.Globalization;
using ClientSamgk;
using GeneratorDuty.BuilderHtml;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository.Duty;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.BackgroundTasks;

public class AutoSendScheduleExport(
    ITelegramBotClient client,
    DutyRepository repository,
    ClientSamgkApi clientSamgkApi,
    ILogger<AutoSendScheduleExport> logger) : BackgroundServiceBase
{
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"Запущен сервис");

        while (!stoppingToken.IsCancellationRequested)
        {
            var dateTime = DateTime.Now;

            if (!CanWorkSerivce(DateTime.Now))
            {
                await Task.Delay(1000);
                continue;
            }

            logger.LogInformation($"Запуск скрипта по расписанию");

            var scheduleProps = await repository.ScheduleProps.GetSchedulePropsFromAutoExport(true);

            if (dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                await Task.Delay(1000);
                return;
            }

            var rules = await repository.ScheduleRules.GetRuleFromDateOrDefault(DateOnly.FromDateTime(dateTime));
            
            foreach (var item in scheduleProps.Where(item => item.LastResult != DateTime.Now.ToString("yyyy-MM-dd")))
            {
                logger.LogInformation($"Скрипт № {item.Id} начал работать");
                var builderSchedule = new HtmlBuilderSchedule();

                var allExportResult = await clientSamgkApi.Schedule
                    .GetAllScheduleAsync(DateOnly.FromDateTime(dateTime), item.SearchType, rules.CallType, rules.ShowImportantLesson, rules.ShowRussianHorizont, delay: 1500);

                if (allExportResult.Count is 0)
                {
                    logger.LogInformation(
                        $"Скрипт № {item.Id} отработан: Расписание на {dateTime.ToString(CultureInfo.InvariantCulture)} - 0 пар");
                    await Task.Delay(1000);
                    continue;
                }

                foreach (var scheduleFromDate in allExportResult)
                    builderSchedule.AddRow(scheduleFromDate, item.SearchType);

                var success = await client.TrySendDocument(item.IdPeer,
                    new InputFileStream(builderSchedule.GetStreamFile(),
                        $"{DateOnly.FromDateTime(dateTime)}_{item.SearchType}.html"));

                if (!success)
                {
                    item.Fails++;
                    logger.LogInformation($"Скрипт № {item.Id} не отработан: Ошибки при отправке сообщения");
                }

                if (success)
                {
                    item.Fails = 0;
                    item.LastResult = DateTime.Now.ToString("yyyy-MM-dd");
                    logger.LogInformation($"Скрипт № {item.Id} отработан: ОК");
                }

                await repository.ScheduleProps.Update(item);
                /*if (item.Fails >= 10)
                {
                    logger.LogInformation($"Скрипт № {item.Id} удалена из за большого кол-во ошибок");
                    await repository.ScheduleProps.Remove(item);
                }*/

                logger.LogInformation($"Скрипт № {item.Id} Завершен");
            }
            
            await Task.Delay(1800000);
        }
    }
}