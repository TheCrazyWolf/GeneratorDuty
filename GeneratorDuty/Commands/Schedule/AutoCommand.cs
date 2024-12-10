using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using GeneratorDuty.Repository.Duty;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands.Schedule;

public class AutoCommand(DutyRepository repository) : BaseCommand
{
    public override string Command { get; } = "/auto";

    private readonly string _usage =
        "ℹ️ На эту беседу нет установленных данных. Используйте /set <фио препода/группа/кабинет>";

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
        
        var widget = await repository.MessageWidgets.GetWidgetByChatIdAsync(prop.IdPeer);
        
        if (widget is not null)
        {
            await client.TrySendMessage(message.Chat.Id, $"ℹ️ Вы не можете использовать авто-отправку, с активными виджетами. Оключите /widget, затем активируйте /auto");
            return;
        }
        
        await repository.ScheduleProps.UpdateAutoSend(prop,!prop.IsAutoSend);

        string messageNotify = prop.IsAutoSend
            ? "✅ Авто-рассылка расписания включена"
            : "ℹ️ Авто-рассылка расписания выключена";

        await client.TrySendMessage(message.Chat.Id, messageNotify);
    }
}