using GeneratorDuty.Common;
using GeneratorDuty.CustomRights;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models;
using GeneratorDuty.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.CallBackKeyboards.Duty;

public class DutyForce(DutyRepository repository) : CallQuery
{
    public override string Name { get; set; } = "duty_force";

    public override async void Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
    {
        var array = TryGetArrayFromCallBack(callbackQuery);
        if (callbackQuery.Message is null || array is null || array.Length == 0 ||
            !long.TryParse(array[0], out var idMemberDuty)) return;
        
        if (Restrictions.ChatIdsRequiredAdminRights.Contains(callbackQuery.Message.Chat.Id) && !await client.IsUserAdminInChat(callbackQuery.From.Id, callbackQuery.Message.Chat.Id))
        {
            await client.AnswerCallbackQueryAsync(callbackQuery.Id, "❌ \n\nВ этом чате данное действие могут выполнять только админы беседы", true);
            return;
        }
        
        var members = await repository.Members.GetMemberDuty(idMemberDuty);
        if (members is null) return;

        await repository.LogsMembers.Create(new LogDutyMember
        {
            UserId = members.Id,
            Date = DateTime.Now
        });
        
        foreach (var member in await repository.LogsMemberPriority.GetLogsByIdMember(idMemberDuty))
            await repository.LogsMemberPriority.Remove(member);
        
        await client.TryDeleteMessage(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
        await client.TrySendMessage(callbackQuery.Message.Chat.Id,$"Назначен дежурный вручную: {members.MemberNameDuty}");
    }
}