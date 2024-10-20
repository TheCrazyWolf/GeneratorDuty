using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands;

public class AutoCommand(DutyRepository repository) : BaseCommand
{
    public override string Command { get; } = "/auto";

    private readonly string _usage = "ℹ️ На эту беседу нет установленных данных. Используйте /set <фио препода/группа/кабинет>";
    private readonly string _success = "✅ Настройки этой беседы обновлены";
    private readonly string _unSuccess = "ℹ️ Не удалось обновить настройки. Usage: /auto true/false";
    
    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        message.Text = message.Text.GetReplacedCommandFromDomain();

        var prop = await repository.ScheduleProps.GetSchedulePropFromChat(message.Chat.Id);

        if (prop is null)
        {
            await client.TrySendMessage(message.Chat.Id, _usage);
            return;
        }

        if (!bool.TryParse(message.Text.Split(' ')[1], out bool result))
        {
            await client.TrySendMessage(message.Chat.Id, _unSuccess);
            return;
        }
        
        prop.IsAutoSend = result;
        await repository.ScheduleProps.Update(prop);
        
        await client.TrySendMessage(message.Chat.Id, _success);
    }
}