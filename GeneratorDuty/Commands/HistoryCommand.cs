using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Models;
using GeneratorDuty.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.Commands;

public class HistoryCommand(DutyContext ef) : BaseCommand
{
    public static MemoryExceptionDuty Cache { get; set; } = new ();
    
    public override string Command { get; } = "/history";
    
    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;
        
        var mainList = await ef.MemberDuties
            .Where(x => x.IdPeer == message.Chat.Id)
            .ToListAsync();

        string messageToBeSend = "Студент | Дата дежурства\n";
        
        foreach (var item in mainList)
        {
            var inHistory = await FoundInHistory(item);
            string date = inHistory is null ? "Н/Д" : inHistory.Date.ToString("dd.MM.yyyy");
            messageToBeSend += $"{item.MemberNameDuty} | {date}\n";
        }
        
        await client.SendTextMessageAsync(message.Chat.Id, messageToBeSend);
    }
    
    private async Task<LogDutyMember?> FoundInHistory(MemberDuty duty)
    {
        return await ef.LogDutyMembers
            .Where(x => x.UserId == duty.Id)
            .OrderBy(x=> x.Date).LastOrDefaultAsync();
    }
}