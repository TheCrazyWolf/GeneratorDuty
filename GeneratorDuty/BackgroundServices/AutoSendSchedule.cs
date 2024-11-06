﻿using System.Timers;
using ClientSamgk;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using GeneratorDuty.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace GeneratorDuty.BackgroundServices;

public class AutoSendSchedule(ITelegramBotClient client, DutyRepository repository, 
    ClientSamgkApi clientSamgkApi, ILogger<AutoSendSchedule> logger) : BackgroundServiceBase
{
    private async void OnEventExecution(object? sender, ElapsedEventArgs e)
    {
        var dateTime = DateTime.Now;
        
        if(!CanWorkSerivce(DateTime.Now)) return;
        
        logger.LogInformation($"Запуск скрипта по расписанию");
        
        var scheduleProps = await repository.ScheduleProps.GetSchedulePropsFromAutoSend(true);
        
        // если время вечернее смотрим расписание на перед
        if (dateTime.Hour >= 10) dateTime = dateTime.AddDays(1);
            
        // если день выходной, то пропускаем и добавляем дни пока не попадется рабочий
        while (dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) dateTime = dateTime.AddDays(1);
        
        foreach (var item in scheduleProps)
        {
            logger.LogInformation($"Скрипт № {item.Id} начал работать");
            
            var result = await clientSamgkApi.Schedule
                .GetScheduleAsync(DateOnly.FromDateTime(dateTime), item.SearchType, item.Value);

            if (result.Lessons.Count is 0) continue;

            var md5New = result.GetMd5();
            
            if (item.LastResult == md5New) continue;
                
            var success = await client.TrySendMessage(item.IdPeer, result.GetStringFromRasp());

            if (!success) item.Fails++;
            
            if (success)
            {
                item.Fails = 0;
                item.LastResult = md5New;
                logger.LogInformation($"Скрипт № {item.Id} отработан");
            }
            
            await repository.ScheduleProps.Update(item);
            if (item.Fails >= 10) await repository.ScheduleProps.Remove(item);
            
            await Task.Delay(1000);
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Timer.Elapsed += OnEventExecution;
        Timer.Start();
        logger.LogInformation($"Запущен сервис");
        return Task.CompletedTask;
    }
}