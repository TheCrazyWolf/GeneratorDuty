using GeneratorDuty.Common;
using GeneratorDuty.Database;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands;

public class GetCommand(DutyContext ef) : BaseCommand
{
    readonly Random _rnd = new Random();
    public override string Command { get; } = "/get";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;
        
        var members = await ef.MemberDuties.Where(x => x.IdPeer == message.Chat.Id).ToListAsync();

        if (members.Count is 0)
        {
            await client.SendTextMessageAsync(message.Chat.Id, $"ℹ️ В это беседе не настроены списки дежурных, юзай /update и укажи список группы разделяя переносом строки ФИО");
            return;
        }
        
        string duntyStudent = members[_rnd.Next(0, members.Count)].MemberNameDuty;
        
        await client.SendTextMessageAsync(message.Chat.Id, $"✅ Сегодня дежурит: {duntyStudent}");
    }
}