using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using GeneratorDuty.Repository.Duty;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands.Duty;

public class RestrictionPeer(DutyRepository repository) : BaseCommand
{
    public override string Command { get; } = "/requireadmin";
    
    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        message.Text = message.Text.GetReplacedCommandFromDomain();

        var prop = await repository.ScheduleProps.GetSchedulePropFromChat(message.Chat.Id);

        if(prop is null)
        {
            await client.TrySendMessage(message.Chat.Id, $"ℹ️ Не смог найти настройки для Вашей беседы, задай /set <группа, фио препода, кабинет>");
            return;
        }

        if (!await client.IsUserAdminInChat(message.From.Id, message.Chat.Id))
        {
            await client.TrySendMessage(message.Chat.Id, $"ℹ️ Это действие могут выполнить только администраторы чата");
            return;
        }
        
        prop.IsRequiredAdminRights = !prop.IsRequiredAdminRights;
        await repository.ScheduleProps.Update(prop);

        string messageNotify = prop.IsRequiredAdminRights
            ? "✅ Часть функционала ограничена будет доступна только администраторам: (подтверждения дежурного)"
            : "ℹ️ Ограничения в чате сняты";
        
        
        await client.TrySendMessage(message.Chat.Id, messageNotify);
    }
}