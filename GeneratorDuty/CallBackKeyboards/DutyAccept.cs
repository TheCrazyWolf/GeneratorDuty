using GeneratorDuty.Database;
using GeneratorDuty.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.CallBackKeyboards;

public class DutyAccept(DutyContext ef) : CallQuery
{
    public override string Name { get; set; } = "duty_accept";

    public override async void Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
    {
        var array = TryGetArrayFromCallBack(callbackQuery);
        if (callbackQuery.Message is null || array is null || array.Length == 0 ||
            !long.TryParse(array[0], out var idMemberDuty)) return;

        var memberDuty = await ef.MemberDuties.FirstOrDefaultAsync(x => x.Id == idMemberDuty);
        if (memberDuty is null) return;

        await ef.AddAsync(new LogDutyMember
        {
            UserId = memberDuty.Id,
            Date = DateTime.Now
        });
        await ef.SaveChangesAsync();
        await Amnesty(memberDuty.Id);

        await client.EditMessageReplyMarkupAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
            replyMarkup: null);
    }

    private async Task Amnesty(long idMember)
    {
        var fails = await ef.LogDutyMemberLosts.Where(x => x.UserId == idMember).ToListAsync();

        foreach (var item in fails)
        {
            ef.Remove(item);
        }

        await ef.SaveChangesAsync();
    }
}