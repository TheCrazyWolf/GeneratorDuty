using ClientSamgk;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository.Duty;
using GeneratorDuty.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace GeneratorDuty.BackgroundTasks;

public class AutoPinMessageWithSchedule(
    ITelegramBotClient client,
    DutyRepository repository,
    ClientSamgkApi clientSamgkApi,
    ILogger<AutoPinMessageWithSchedule> logger) : BackgroundServiceBase
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"Запущен сервис");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!CanWorkSerivce(DateTime.Now))
            {
                await Task.Delay(1000);
                continue;
            }

            logger.LogInformation($"Запуск скрипта по расписанию");

            var scheduleProps = await repository.ScheduleProps.GetAllProps();

            DateOnly todayDate = DateOnly.FromDateTime(DateTime.Now);

            foreach (var item in scheduleProps)
            {
                var pinnedMessage = await repository.MessageWidgets.GetWidgetByChatIdAsync(item.IdPeer);
                if (pinnedMessage == null) continue;

                string builderMessage = string.Empty;

                string changedDates = string.Empty;
                
                for (int i = 0; i <= 5; i++)
                {
                    // если день выходной, то пропускаем и добавляем дни пока не попадется рабочий
                    while (todayDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) todayDate = todayDate.AddDays(1);

                    var rules = await repository.ScheduleRules.GetRuleFromDateOrDefault(todayDate);
                    
                    var history = await repository.ScheduleHistory.CreateOrGetScheduleHistory(item.IdPeer, todayDate);

                    var result = await clientSamgkApi.Schedule.GetScheduleAsync(todayDate, item.SearchType,
                        item.Value, rules.CallType, rules.ShowImportantLesson, rules.ShowRussianHorizont);

                    builderMessage += $"{result.GetStringFromRasp()}";
                    
                    if (history.Result != result.GetStringFromRasp())
                    {
                        changedDates += $"{result.Date.ToString()}, ";
                        history.Result = result.GetStringFromRasp();
                    }

                    await repository.ScheduleProps.Update(item);
                    await repository.ScheduleHistory.UpdateScheduleHistory(history);
                    todayDate = todayDate.AddDays(1);
                }

                builderMessage += $"Последнее обновление: {DateTime.Now} DateTime.Now.ToString(CultureInfo.CurrentCulture)}}";
                var success = await client.TryEditMessage(pinnedMessage.ChatId, pinnedMessage.MessageId, builderMessage);
                if (!string.IsNullOrEmpty(changedDates)) await client.TrySendMessage(pinnedMessage.ChatId, $"Обратите внимание на изменения в расписании: {changedDates}");
                
                if (!success)
                {
                    await client.TryDeleteMessage(pinnedMessage.ChatId, pinnedMessage.MessageId);
                    await client.TryUnPinMessage(pinnedMessage.ChatId, pinnedMessage.MessageId);
                    var message = await client.TrySendMessage(pinnedMessage.ChatId, builderMessage);
                    if (message is not null)
                    {
                        await repository.MessageWidgets.UpdateMessageWidgetMessageId(pinnedMessage, message.Chat.Id, message.MessageId);
                        await client.TryPinMessage(message.Chat.Id, message.MessageId);
                    }
                }
                await Task.Delay(1500);
            }

            await Task.Delay(3000);
        }
    }
}