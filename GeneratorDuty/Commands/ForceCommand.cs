using GeneratorDuty.Common;
using GeneratorDuty.CustomRights;
using GeneratorDuty.Database;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models;
using GeneratorDuty.Repository;
using GeneratorDuty.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.Commands;

public class ForceCommand(DutyRepository repository) : BaseCommand
{
    public override string Command { get; } = "/force";

    private readonly string _usage =
        $"ℹ️ В это беседе не настроены списки дежурных, юзай /update и укажи список группы разделяя переносом строки ФИО";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;
        
        if (Restrictions.ChatIdsRequiredAdminRights.Contains(message.Chat.Id) && !await client.IsUserAdminInChat(message.From.Id, message.Chat.Id))
        {
            await client.TrySendMessage(message.Chat.Id, "В этом чате данное действие могут выполнять только админы беседы");
            return;
        }
        
        var mainList = (await repository.Members.GetMembersFromChat(message.Chat.Id)).ToList();

        if (mainList.Count is 0)
        {
            await client.TrySendMessage(message.Chat.Id, _usage);
            return;
        }

        string msg = "Выберите дежурного вручную: ";

        await client.TrySendMessage(message.Chat.Id, msg,
            replyMarkup: new InlineKeyboardMarkup(GenerareKeyboard(mainList)));
    }

    private IList<IList<InlineKeyboardButton>> GenerareKeyboard(List<MemberDuty> members)
    {
        var keyboard = new List<IList<InlineKeyboardButton>>();
        var row = new List<InlineKeyboardButton>(); 
        int buttonCount = 0;

        foreach (var member in members)
        {
            if (buttonCount == 3)
            {
                keyboard.Add(row); 
                row = new List<InlineKeyboardButton>();
                buttonCount = 0;
            }

            row.Add(InlineKeyboardButton.WithCallbackData($"{member.MemberNameDuty}", $"duty_force {member.Id}"));
            buttonCount++;
        }

        if (row.Count != 0) 
        {
            keyboard.Add(row);
        }
        
        row.Add(InlineKeyboardButton.WithCallbackData($"🔸Отмена", $"duty_force_cancel"));
        return keyboard;
    }
}