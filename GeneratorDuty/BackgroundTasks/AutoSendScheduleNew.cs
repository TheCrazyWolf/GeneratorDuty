using ClientSamgk;
using GeneratorDuty.Repository.Duty;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace GeneratorDuty.
    BackgroundTasks;

public class AutoSendScheduleNew(
    ITelegramBotClient client,
    DutyRepository repository,
    ClientSamgkApi clientSamgkApi,
    ILogger<AutoSendSchedule> logger) : AutoSendSchedule(client, repository, clientSamgkApi, logger)
{
    public override int DaysCanBeChecked { get; set; } = 5;
    public override bool IsMigrated { get; set; } = true;
}