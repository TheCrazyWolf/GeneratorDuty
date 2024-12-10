using ClientSamgk;
using ClientSamgkOutputResponse.Enums;
using GeneratorDuty.BuilderHtml;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands.Schedule;

public class ExportCommand(ClientSamgkApi clientSamgk) : BaseCommand
{
    public override string Command { get; } = "/export";

    private readonly string _usage = "Usage: /export <type> <date>";
    private readonly string _wait = "Запрос принят, ожидайте результата";
    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        message.Text = message.Text.GetReplacedCommandFromDomain().Replace(Command, string.Empty);

        DateTime dateTime;
        ScheduleSearchType searchType = ScheduleSearchType.Cab;
        
        try
        {
            var array = message.Text.Split(' ');
            searchType = ConvertToType(array[1]);
            dateTime = Convert.ToDateTime(array[2]);
        }
        catch 
        {
            await client.TrySendMessage(message.Chat.Id, _usage);
            return;
        }
        
        await client.SendTextMessageAsync(message.Chat.Id, _wait);
        
        var result = await clientSamgk.Schedule.GetAllScheduleAsync(DateOnly.FromDateTime(dateTime), searchType);

        HtmlBuilderSchedule builderSchedule = new HtmlBuilderSchedule();
        
        foreach (var item in result)
            builderSchedule.AddRow(item, searchType);

        await client.SendDocumentAsync(message.Chat.Id,
            new InputFileStream(builderSchedule.GetStreamFile(), $"{DateOnly.FromDateTime(dateTime)}.html"));
    }


    private ScheduleSearchType ConvertToType(string type)
    {
        if (Enum.TryParse(type,true, out ScheduleSearchType searchType))
            return searchType;
        
        throw new Exception("Fail to convert " + type + " to ScheduleSearchType");
    }
}