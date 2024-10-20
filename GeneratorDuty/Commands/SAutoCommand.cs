using ClientSamgk.Enums;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models;
using GeneratorDuty.Repository;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands;

public class SAutoCommand(DutyRepository ef) : BaseCommand
{
    public override string Command { get; } = "/sauto";
    

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        message.Text = message.Text.GetReplacedCommandFromDomain().Replace(Command, string.Empty);

        if (message.Text.Contains("clear"))
        {
            foreach (var prop in await ef.ScheduleProps.GetSchedulePropsFromAutoExport(true))
                await ef.ScheduleProps.Remove(prop);
            
            await client.TrySendMessage(message.Chat.Id, "ℹ️ Специальные сценарии очищены");
            return;
        }
        
        try
        {
            var array = message.Text.Split(' ');
            var searchType = ConvertToType(array[1]);
            await ef.ScheduleProps.Create(new ScheduleProp { IdPeer = message.Chat.Id, IsAutoExport = true, SearchType = searchType} );
            
            await client.TrySendMessage(message.Chat.Id, $"✅ К этой беседе добавили автоэкспорт: {searchType}");
        }
        catch 
        {
            await client.TrySendMessage(message.Chat.Id, "ℹ️ Usage: /sauto <type>");
        }
        
    }
    
    private ScheduleSearchType ConvertToType(string type)
    {
        if (Enum.TryParse(type, out ScheduleSearchType searchType))
            return searchType;
        
        throw new Exception("Fail to convert " + type + " to ScheduleSearchType");
    }
}