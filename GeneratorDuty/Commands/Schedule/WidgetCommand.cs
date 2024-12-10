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

        var widget = await repository.MessageWidgets.GetMessageWidgetsAsync(prop.IdPeer);

        if (widget.Any())
        {
            await client.TrySendMessage(message.Chat.Id, $"ℹ️ Виджет с расписанием отключен");

            foreach (var item in widget)
            {
                await client.TryUnPinMessage(item.ChatId, item.MessageId);
                await client.TryDeleteMessage(item.ChatId, item.MessageId);
                await repository.MessageWidgets.DeleteMessageWidgetAsync(item);
            }
            
            return;
        }

        if (prop.IsAutoSend)
        {
            await client.TrySendMessage(message.Chat.Id, $"ℹ️ Вы не можете использовать виджет-режим с авто-отправкой. Отключите /auto, прежде чем активировать виджет.");
            return;
        }
        
        var scheduleProp = await client.TrySendMessage(prop.IdPeer, "✅ Это сообщение будет автоматически обновляться с расписанием на 5 дней вперед. Это может занять 5-15 минут, ожидайте. Если активировали виджет ночью, дождитесь обновления на утро.");
        
        if(scheduleProp is null) return;
        
        var messageWidget = await repository.MessageWidgets.CreateMessageWidgetAsync(new MessageWidget() { ChatId = scheduleProp.Chat.Id, MessageId = scheduleProp.MessageId });
        
        if (!await client.TryPinMessage(scheduleProp.Chat.Id, scheduleProp.MessageId))
        {
            await client.TryEditMessage(messageWidget.ChatId, messageWidget.MessageId, "⚠️ Не удалось настроить виджет. Необходимы права администратора чата для: редактирования сообщений, закрепления.");
            await repository.MessageWidgets.DeleteMessageWidgetAsync(messageWidget);
        }
    }
}