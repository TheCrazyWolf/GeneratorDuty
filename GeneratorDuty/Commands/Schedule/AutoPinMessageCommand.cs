using ClientSamgk;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models.Schedule;
using GeneratorDuty.Repository;
using GeneratorDuty.Repository.Duty;
using GeneratorDuty.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.Commands.Schedule;

public class AutoPinMessageCommand(DutyRepository repository) : BaseCommand
{
    public override string Command { get; } = "";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        /*if (message.Type is not MessageType.MessagePinned || message.PinnedMessage is null) return;

        var widget = await repository.MessageWidgets.GetWidgetByChatIdAsync(message.Chat.Id);
        
        if(widget is null) return;

        await client.TryUnPinMessage(message.PinnedMessage.Chat.Id, message.PinnedMessage.MessageId);
        await client.TrySendMessage(message.Chat.Id,
            "К сожалению, в этом режиме не получится держать несколько закрепленных сообщений");*/
    }
}