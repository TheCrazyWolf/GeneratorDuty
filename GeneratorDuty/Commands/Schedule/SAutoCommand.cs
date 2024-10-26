using ClientSamgk.Enums;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models;
using GeneratorDuty.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands.Schedule;

public class SAutoCommand(DutyRepository repository) : BaseCommand
{
    public override string Command { get; } = "/sauto";
    

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        message.Text = message.Text.GetReplacedCommandFromDomain().Replace(Command, string.Empty);

        if (message.Text.Contains("clear"))
        {
            foreach (var prop in await repository.ScheduleProps.GetSchedulePropsFromAutoExport(true))
                await repository.ScheduleProps.Remove(prop);
            
            await client.TrySendMessage(message.Chat.Id, "ℹ️ Специальные сценарии очищены");
            return;
        }
        
        try
        {
            var array = message.Text.Split(' ');
            var searchType = ConvertToType(array[1]);
            await repository.ScheduleProps.Create(new ScheduleProp { IdPeer = message.Chat.Id, IsAutoExport = true, SearchType = searchType} );
            
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