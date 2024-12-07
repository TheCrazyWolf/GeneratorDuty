using GeneratorDuty.Commands.Duty;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models;
using GeneratorDuty.Repository;
using GeneratorDuty.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.CallBackKeyboards.Duty;

public class DutyReject(DutyRepository repository, MemoryExceptionDuty cache) : CallQuery
{
    public override string Name { get; set; } = "duty_reject";

    public override async void Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
    {
        var array = TryGetArrayFromCallBack(callbackQuery);
        if (callbackQuery.Message is null || array is null || array.Length == 0 ||
            !long.TryParse(array[0], out var idMemberDuty)) return;

        var prop = await repository.ScheduleProps.GetSchedulePropFromChat(callbackQuery.Message.Chat.Id);

        if (prop is null) return;
        
        if (prop.IsRequiredAdminRights && !await client.IsUserAdminInChat(callbackQuery.From.Id, callbackQuery.Message.Chat.Id))
        {
            await client.AnswerCallbackQueryAsync(callbackQuery.Id, "❌ \n\nВ этом чате данное действие могут выполнять только админы беседы", true);
            return;
        }
        
        var memberDuty = await repository.Members.GetMemberDuty(idMemberDuty);
        if (memberDuty is null) return;

        cache.AddMemberDuty(memberDuty);

        await repository.LogsMemberPriority.Create(new LogDutyMemberPriority
        {
            UserId = memberDuty.Id,
        });

        await client.TrySendMessage(callbackQuery.Message.Chat.Id,
            $"О как.. Я запомнил, что {memberDuty.MemberNameDuty} сегодня нет. В следующий раз, заставлю отдежурить 😈");
        
        await client.TryDeleteMessage(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);

        GetCommand command = new(repository, cache);
        await command.ExecuteAsync(client,callbackQuery.Message);
    }
}