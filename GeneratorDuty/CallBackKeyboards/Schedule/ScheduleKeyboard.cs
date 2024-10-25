using ClientSamgk;
using ClientSamgk.Enums;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.CallBackKeyboards.Schedule;

public class ScheduleKeyboard(ClientSamgkApi clientSamgk) : CallQuery
{
    public override string Name { get; set; } = "schedule";

    public override async void Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
    {
        var array = TryGetArrayFromCallBack(callbackQuery);
        
        if (callbackQuery.Message is null || array is null || array.Length == 0 ||
            !Enum.TryParse<ScheduleSearchType>(array[0], out var searchType) || !DateTime.TryParse(array[2], out var date))
        {
            if (callbackQuery.Message != null)
                await client.EditMessageReplyMarkupAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                    replyMarkup: null);
            return;
        }
        
        var result = await clientSamgk.Schedule.GetScheduleAsync(DateOnly.FromDateTime(date), 
            searchType, array[1]);
        
        await client.TryEditMessage(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, 
            result.GetStringFromRasp(),
            replyMarkup: new InlineKeyboardMarkup(result.GenerateKeyboardOnSchedule(searchType, array[1])));
        
    }
}