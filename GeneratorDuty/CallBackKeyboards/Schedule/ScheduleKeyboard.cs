using ClientSamgk;
using ClientSamgkOutputResponse.Enums;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models.Schedule;
using GeneratorDuty.Repository.Duty;
using GeneratorDuty.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.CallBackKeyboards.Schedule;

public class ScheduleKeyboard(ClientSamgkApi clientSamgk, DutyRepository repository) : CallQuery
{
    public override string Name { get; set; } = "schedule";

    public override async void Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
    {
        var array = TryGetArrayFromCallBack(callbackQuery);
        
        //  schedule <type> <value> <date>
        if (callbackQuery.Message is null || array is null || array.Length == 0 ||
            !Enum.TryParse<ScheduleSearchType>(array[0], out var searchType) || !DateTime.TryParse(array[2], out var date))
        {
            if (callbackQuery.Message != null)
                await client.TryDeleteMessage(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
            return;
        }
        
        var rules = await repository.ScheduleRules.GetRuleFromDateOrDefault(DateOnly.FromDateTime(date));
        
        if (date < DateTime.Now.Date)
        {
            var cached =
                await repository.ScheduleHistory.GetScheduleHistory(callbackQuery.Message.Chat.Id,
                    DateOnly.FromDateTime(date));

            if (cached is not null && !string.IsNullOrEmpty(cached.Result))
            {
                await client.TryEditMessage(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, 
                    cached.Result,
                    replyMarkup: new InlineKeyboardMarkup(GenerateKeyboardOnSchedule(date, searchType, array[1])));
                return;
            }
        }
        
        var result = await clientSamgk.Schedule.GetScheduleAsync(DateOnly.FromDateTime(date), 
            searchType, array[1], rules.CallType, rules.ShowImportantLesson, rules.ShowRussianHorizont);

        if (date < DateTime.Now.Date)
        {
            await repository.ScheduleHistory.CreateScheduleHistory(new ScheduleHistory()
                        { ChatId = callbackQuery.Message.Chat.Id, Date = DateOnly.FromDateTime(date), Result = result.GetStringFromRasp() });
        }
        
        await client.TryEditMessage(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, 
            result.GetStringFromRasp(),
            replyMarkup: new InlineKeyboardMarkup(result.GenerateKeyboardOnSchedule(searchType, array[1])));
    }
    
    
    public static IList<IList<InlineKeyboardButton>> GenerateKeyboardOnSchedule(DateTime date, ScheduleSearchType type, string value)
    {
        // schedule <type> <value> <date>
        return new List<IList<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("👈",
                    $"schedule {type} {value} {date.Date.AddDays(-1):dd.MM.yyyy}"),
                InlineKeyboardButton.WithCallbackData("❌",
                    $"schedule clear"),
                InlineKeyboardButton.WithCallbackData("♻️",
                    $"schedule {type} {value} {date.Date:dd.MM.yyyy}"),
                InlineKeyboardButton.WithCallbackData("👉",
                    $"schedule {type} {value} {date.Date.AddDays(+1):dd.MM.yyyy}"),
            },
        };
    }
}