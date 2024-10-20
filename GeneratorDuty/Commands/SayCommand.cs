using ClientSamgk;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using GeneratorDuty.Utils;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GeneratorDuty.Commands;

public class SayCommand(DutyRepository repository) : BaseCommand
{
    private readonly long _idAdmin = 208049718;
    public override string Command { get; } = "/say";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null || message.From.Id != _idAdmin) return;

        message.Text = message.Text.GetReplacedCommandFromDomain().Replace(Command, string.Empty);

        var props = await repository.ScheduleProps.GetScheduleProps();

        foreach (var scheduleProp in props)
        {
            await client.TrySendMessage(scheduleProp.IdPeer, message.Text);
            await Task.Delay(1000);
        }
        
    }
}