using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models;
using GeneratorDuty.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.CallBackKeyboards;

public class DutyAccept(DutyRepository repository) : CallQuery
{
    public override string Name { get; set; } = "duty_accept";

    public override async void Execute(ITelegramBotClient client, CallbackQuery callbackQuery)
    {
        var array = TryGetArrayFromCallBack(callbackQuery);
        if (callbackQuery.Message is null || array is null || array.Length == 0 ||
            !long.TryParse(array[0], out var idMemberDuty)) return;

        if (!await client.IsUserAdminInChat(callbackQuery.From.Id, callbackQuery.Message.Chat.Id))
        {
            await client.AnswerCallbackQueryAsync(callbackQuery.Id, "❌ \n\nВы точно админ? Вас не узнали... Эта кнопка работает только в чатах и Вы должны быть админом", true);
            return;
        }

        var memberDuty = await repository.Members.GetMemberDuty(idMemberDuty);
        if (memberDuty is null) return;

        await repository.LogsMembers.Create(new LogDutyMember
        {
            UserId = memberDuty.Id,
            Date = DateTime.Now
        });
        
        foreach (var member in await repository.LogsMemberPriority.GetLogsByIdMember(idMemberDuty))
            await repository.LogsMemberPriority.Remove(member);
        
        await client.EditMessageReplyMarkupAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
            replyMarkup: null);
    }
}