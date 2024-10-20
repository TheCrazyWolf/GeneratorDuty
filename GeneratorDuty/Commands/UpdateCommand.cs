using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands;

public class UpdateCommand(DutyContext ef) : BaseCommand
{
    public override string Command { get; } = "/update";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;
        message.Text = message.Text.GetReplacedCommandFromDomain().Replace(Command, string.Empty);

        var membersArray = message.Text.Split('\n');

        if (membersArray.Length is 0 or 1)
        {
            await client.SendTextMessageAsync(message.Chat.Id, $"Слишком короткий список");
            return;
        }
        
        await GetAndRemoveOlds(message.Chat.Id);
        var countResult = await AddNewDuty(membersArray, message.Chat.Id);
        await client.SendTextMessageAsync(message.Chat.Id, $"✅ Обновили список группы: {countResult}");
    }

    private async Task GetAndRemoveOlds(long peerId)
    {
        foreach (var item in await ef.MemberDuties
                     .Where(x=> x.IdPeer == peerId)
                     .ToListAsync())
        {
            ef.Remove(item);
        }

        await ef.SaveChangesAsync();
    }

    private async Task<int> AddNewDuty(IEnumerable<string> d, long peerId)
    {
        int count = 0;
        foreach (var item in d)
        {
            if(string.IsNullOrEmpty(item)) continue;
            
            await ef.AddAsync(new MemberDuty
            {
                IdPeer = peerId,
                MemberNameDuty = item
            });
            count++;
        }

        await ef.SaveChangesAsync();

        return count;
    }
}