using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.CallBackKeyboards.Duty;

public class DutyForceCancel(DutyRepository repository) : CallQuery
{
    public override string Name { get; set; } = "duty_force_cancel";

    public override async void Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
    {
        var array = TryGetArrayFromCallBack(callbackQuery);
        if (callbackQuery.Message is null || array is null) return;
        
        await client.TryDeleteMessage(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
    }
}