using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Models;
using GeneratorDuty.Services;
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

        await GetAndRemoveOlds(message.From.Id);
        await AddNewDuty(message.Text.Split('\n'), message.From.Id);
        await client.SendTextMessageAsync(message.From.Id, "Готово!");
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

    private async Task AddNewDuty(IEnumerable<string> d, long peerId)
    {
        foreach (var item in d)
        {
            if(string.IsNullOrEmpty(item)) continue;
            
            await ef.AddAsync(new MemberDuty
            {
                IdPeer = peerId,
                MemberNameDuty = item
            });
        }

        await ef.SaveChangesAsync();
    }
}