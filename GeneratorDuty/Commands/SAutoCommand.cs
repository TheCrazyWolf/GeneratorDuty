using ClientSamgk.Enums;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands;

public class SAutoCommand(DutyContext ef) : BaseCommand
{
    public override string Command { get; } = "/sauto";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        message.Text = message.Text.GetReplacedCommandFromDomain().Replace(Command, string.Empty);

        if (message.Text.Contains("clear"))
        {
            await client.SendTextMessageAsync(message.Chat.Id, "ℹ️ Специальные сценарии очищены");
            await GetAndRemoveOlds(message.Chat.Id);
            return;
        }
        
        try
        {
            var array = message.Text.Split(' ');
            var searchType = ConvertToType(array[1]);
            await ef.AddAsync(new ScheduleProp { IdPeer = message.Chat.Id, IsAutoExport = true, SearchType = searchType} );
            await ef.SaveChangesAsync();
            await client.SendTextMessageAsync(message.Chat.Id, $"✅ К этой беседе добавили автоэкспорт: {searchType}");
        }
        catch 
        {
            await client.SendTextMessageAsync(message.Chat.Id, "ℹ️ Usage: /sauto <type>");
        }
        
    }
    
    private async Task GetAndRemoveOlds(long peerId)
    {
        foreach (var item in await ef.ScheduleProps
                     .Where(x => x.IdPeer == peerId && x.IsAutoExport)
                     .ToListAsync())
        {
            ef.Remove(item);
        }

        await ef.SaveChangesAsync();
    }
    
    private ScheduleSearchType ConvertToType(string type)
    {
        if (Enum.TryParse(type, out ScheduleSearchType searchType))
            return searchType;
        
        throw new Exception("Fail to convert " + type + " to ScheduleSearchType");
    }
}