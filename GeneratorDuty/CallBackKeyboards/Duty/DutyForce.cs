using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models;
using GeneratorDuty.Models.Duty;
using GeneratorDuty.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.CallBackKeyboards.Duty;

public class DutyForce(DutyRepository repository) : CallQuery
{
    public override string Name { get; set; } = "duty_force";

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
        
        var members = await repository.Members.GetMemberDuty(idMemberDuty);
        if (members is null) return;

        await repository.LogsMembers.Create(new LogDutyMember
        {
            UserId = members.Id,
            Date = DateTime.Now
        });
        
        foreach (var member in await repository.LogsMemberPriority.GetLogsByIdMember(idMemberDuty))
            await repository.LogsMemberPriority.Remove(member);
        
        await client.TryEditMessage(callbackQuery.Message.Chat.Id,callbackQuery.Message.MessageId, $"Назначен дежурный вручную: {members.MemberNameDuty}", replyMarkup: new ReplyKeyboardRemove());
    }
}