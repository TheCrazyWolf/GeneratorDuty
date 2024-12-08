using ClientSamgkOutputResponse.Enums;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models.Schedule;
using GeneratorDuty.Repository;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands.Schedule;

public class RuleCommand(DutyRepository repository) : BaseCommand
{
    private readonly long _idAdmin = 208049718;
    public override string Command { get; } = "/rule";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null || message.From.Id != _idAdmin) return;

        var array = message.Text.GetReplacedCommandFromDomain().Replace(Command, string.Empty).Split(" ");

        if (!Enum.TryParse<ScheduleCallType>(array[2], true, out var callType) || !DateTime.TryParse(array[1], out var date) 
            || !bool.TryParse(array[3], out var showImportant) || !bool.TryParse(array[4], out var showHorizont ))
        {
            
            var values = Enum.GetValues(typeof(ScheduleCallType)).Cast<ScheduleCallType>();
            string build = string.Join(", ", values);
            await client.TrySendMessage(message.Chat.Id, $"/rule дата тип разо важ гориз \n\n{build}");
            return;
        }

        var rule = await repository.ScheduleRules.GetRuleFromDate(DateOnly.FromDateTime(date));

        if (rule is null)
        {
            await repository.ScheduleRules.CreateRule(new ScheduleCustomRules()
            {
                Date = DateOnly.FromDateTime(date),
                CallType = callType,
                ShowImportantLesson = showImportant,
                ShowRussianHorizont = showHorizont
            });
        }
        else
        {
            rule.CallType = callType;
            rule.ShowImportantLesson = showImportant;
            rule.ShowRussianHorizont = showHorizont;
            await repository.ScheduleRules.UpdateRule(rule);
        }
        await client.TrySendMessage(message.Chat.Id, "Ok");
    }
}