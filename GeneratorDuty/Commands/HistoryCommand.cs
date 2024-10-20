using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models;
using GeneratorDuty.Repository;
using GeneratorDuty.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.Commands;

public class HistoryCommand(DutyRepository repository) : BaseCommand
{
    public override string Command { get; } = "/history";
    
    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;
        
        var mainList = await repository.Members.GetMembersFromChat(message.Chat.Id);

        string messageToBeSend = "Студент | Дата дежурства\n";
        
        foreach (var item in mainList)
        {
            var inHistory = await repository.LogsMembers.GetLastLog(item);
            string date = inHistory is null ? "Н/Д" : inHistory.Date.ToString("dd.MM.yyyy");
            messageToBeSend += $"{item.MemberNameDuty} | {date}\n";
        }
        
        await client.TrySendMessage(message.Chat.Id, messageToBeSend);
    }
}