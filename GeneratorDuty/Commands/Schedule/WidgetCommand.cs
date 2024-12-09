using ClientSamgk;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models.Schedule;
using GeneratorDuty.Repository;
using GeneratorDuty.Repository.Duty;
using GeneratorDuty.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.Commands.Schedule;

public class WidgetCommand(DutyRepository repository) : BaseCommand
{
    public override string Command { get; } = "/widget";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        message.Text = message.Text.GetReplacedCommandFromDomain().Replace(Command, string.Empty);
        
        var prop = await repository.ScheduleProps.GetSchedulePropFromChat(message.Chat.Id);
        
        if(prop is null)
        {
            await client.TrySendMessage(message.Chat.Id, $"ℹ️ Не смог найти настройки для Вашей беседы, задай /set <группа, фио препода, кабинет>");
            return;
        }

        var widget = await repository.MessageWidgets.GetWidgetByChatIdAsync(prop.IdPeer);

        if (widget is not null)
        {
            await client.TrySendMessage(message.Chat.Id, $"ℹ️ Виджет с расписанием отключен");
            await client.TryUnPinMessage(widget.ChatId, widget.MessageId);
            return;
        }
        
        var scheduleProp = await client.TrySendMessage(prop.IdPeer, "эт сообщение будет автоматически обновлено");
        
        if(scheduleProp is null) return;
        
        await repository.MessageWidgets.CreateMessageWidgetAsync(new MessageWidget() { ChatId = scheduleProp.Chat.Id, MessageId = scheduleProp.MessageId });
    }
}