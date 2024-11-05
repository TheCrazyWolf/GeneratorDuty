using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands.Duty;

public class HistoryCommand(DutyRepository repository) : BaseCommand
{
    public override string Command { get; } = "/history";
    
    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;
        
        var mainList = await repository.Members.GetMembersFromChat(message.Chat.Id);

        string messageToBeSend = "Студент | Дата последнего дежурства\n";
        
        foreach (var item in mainList)
        {
            var inHistory = await repository.LogsMembers.GetLastLog(item);
            string date = inHistory is null ? "Н/Д" : inHistory.Date.ToString("dd.MM.yyyy");
            messageToBeSend += $"<b>{item.MemberNameDuty}</b> - {date}\n";
        }
        
        await client.TrySendMessage(message.Chat.Id, messageToBeSend);
    }
}