using ClientSamgk;
using ClientSamgk.Enums;
using GeneratorDuty.BuilderHtml;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GeneratorDuty.Commands;

public class ExportCommand(ClientSamgkApi clientSamgk) : BaseCommand
{
    public override string Command { get; } = "/export";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        HtmlBuilderSchedule builderSchedule = new HtmlBuilderSchedule();
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        message.Text = message.Text.GetReplacedCommandFromDomain().Replace(Command, string.Empty);

        DateTime dateTime = DateTime.Now;
        ScheduleSearchType searchType = ScheduleSearchType.Cab;
        
        try
        {
            var array = message.Text.Split(' ');
            searchType = ConvertToType(array[1]);
            dateTime = Convert.ToDateTime(array[2]);
        }
        catch 
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Usage: /export <type> <date>");
            return;
        }
        
        await client.SendTextMessageAsync(message.Chat.Id, "Запрос принят, ожидайте результата", parseMode: ParseMode.Html);
        
        var result = await clientSamgk.Schedule.GetAllScheduleAsync(DateOnly.FromDateTime(dateTime), searchType);

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