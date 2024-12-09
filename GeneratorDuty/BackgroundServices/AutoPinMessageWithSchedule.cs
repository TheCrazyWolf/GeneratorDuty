using System.Globalization;
using ClientSamgk;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using GeneratorDuty.Repository.Duty;
using GeneratorDuty.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace GeneratorDuty.BackgroundServices;

public class AutoPinMessageWithSchedule(
    ITelegramBotClient client, DutyRepository repository, ClientSamgkApi clientSamgkApi,
    ILogger<AutoPinMessageWithSchedule> logger) : BackgroundServiceBase
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

            var scheduleProps = await repository.ScheduleProps.GetSchedulePropsFromAutoSend(true, isMigrated: true);
            
            DateOnly todayDate = DateOnly.FromDateTime(DateTime.Now);
            
            foreach (var item in scheduleProps)
            {
                var unPinnedMessage = await repository.ScheduleHistory.GetPinnedMessageFromPeerId(item.IdPeer, todayDate);
                foreach (var message in unPinnedMessage)
                {
                    await client.TryUnPinMessage(message.ChatId, message.MessageId);
                    await repository.ScheduleHistory.ChangeStatusPinnedMessage(message, false);
                }
                
                var todayMessage = await repository.ScheduleHistory.GetScheduleHistory(item.IdPeer, todayDate);
                if (todayMessage is not { MessageId: not null, ChatId: not null }) continue;
                
                await client.TryPinMessage(todayMessage.IdPeer, todayMessage.MessageId ?? 0);
                await repository.ScheduleHistory.ChangeStatusPinnedMessage(todayMessage, true);
                await Task.Delay(2000);
            }
            
            await Task.Delay(300000);
        }
    }
}