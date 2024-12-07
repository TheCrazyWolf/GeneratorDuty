using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands.Schedule;

public class MigrateCommand(DutyRepository repository) : BaseCommand
{
    public override string Command { get; } = "/migrate";

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

        prop.IsMigrated = !prop.IsMigrated;
        await repository.ScheduleProps.Update(prop);

        string messageNotify = prop.IsMigrated
            ? "✅ Эта беседа перенесена на новый движок"
            : "ℹ️ Эта беседа возращена на старый движок";

        await client.TrySendMessage(message.Chat.Id, messageNotify);
    }
}