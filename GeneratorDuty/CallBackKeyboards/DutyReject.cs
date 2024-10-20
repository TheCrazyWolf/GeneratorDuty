using GeneratorDuty.Commands;
using GeneratorDuty.Database;
using GeneratorDuty.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
namespace GeneratorDuty.CallBackKeyboards;

public class DutyReject(DutyContext ef) : CallQuery
{
    public override string Name { get; set; } = "duty_reject";

    public override async void Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
    {
        var array = TryGetArrayFromCallBack(callbackQuery);
        if (callbackQuery.Message is null || array is null || array.Length == 0 ||
            !long.TryParse(array[0], out var idMemberDuty)) return;

        var memberDuty = await ef.MemberDuties.FirstOrDefaultAsync(x => x.Id == idMemberDuty);
        if (memberDuty is null) return;

        GetCommand.Cache.AddMemberDuty(memberDuty);
        
        await ef.AddAsync(new LogDutyMemberPriority
        {
            UserId = memberDuty.Id,
        });
        await ef.SaveChangesAsync();

        await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id,
            $"❎ Записал себе в тетрадочку, что {memberDuty.MemberNameDuty} сегодня нет. В следующий раз, он будет в приоритете на дежурству");
        
        await client.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
    }
}