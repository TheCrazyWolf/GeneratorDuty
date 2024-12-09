using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models;
using GeneratorDuty.Models.Duty;
using GeneratorDuty.Repository;
using GeneratorDuty.Repository.Duty;
using GeneratorDuty.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.Commands.Duty;

public class GetCommand(DutyRepository repository, MemoryExceptionDuty cache) : BaseCommand
{
    public override string Command { get; } = "/get";

    private readonly int _daysExcept = 14;
    private readonly string _usage = $"ℹ️ В это беседе не настроены списки дежурных, юзай /update и укажи список группы разделяя переносом строки ФИО";
    
    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;
        
        // главный список группы
        var mainList = (await repository.Members.GetMembersFromChat(message.Chat.Id)).ToList();

        if (mainList.Count is 0)
        {
            await client.TrySendMessage(message.Chat.Id, _usage);
            return;
        }
        
        /* Исключаем из списка тех, кого сейчас нет */

        foreach (var found in cache.GetFromChats(message.Chat.Id)
                     .Select(item => mainList
                         .FirstOrDefault(x => x.Id == item.Id)).OfType<MemberDuty>())
        {
            mainList.Remove(found);
        }
        
        /* ------ */
        
        /* Не пришедшие в прошлый раз */
        
        var memberPriorities = await repository.LogsMemberPriority.GetLogsFromChatId(message.Chat.Id);
        
        foreach (var log in cache.GetFromChats(message.Chat.Id).ToList()
                     .SelectMany(member => memberPriorities.Where(x => x.Duty!.Id == member.Id).ToList()))
        {
            memberPriorities.Remove(log);
        }

        foreach (var member in memberPriorities)
        {
            var foundInHistory = await repository.LogsMembers.FoundInHistory(member.Duty!.Id, _daysExcept);
            if (foundInHistory is not null) continue;
            
            await client.TrySendMessage(message.Chat.Id, $"Я надеюсь, сегодня то {member.Duty?.MemberNameDuty} пришел? За тобой должок остался", replyMarkup: new InlineKeyboardMarkup(GenerareKeyboard(member.Duty!.Id)));
            return;
        }
        
        /* ------ */
        
        /* выборка из тех кто давно не дежурил з */
        
        var listNonDutiesLast = mainList.ToList();
        
        foreach (var member in mainList)
        {
            var item = await repository.LogsMembers.FoundInHistory(member.Id, _daysExcept);
            if(item is null) continue;
            listNonDutiesLast.Remove(member);
        }
        
        Random rnd = new Random();
        
        switch (listNonDutiesLast.Count)
        {
            case 0 when mainList.Count is not 0:
            {
                var memberDutyForce = mainList[rnd.Next(0, mainList.Count)];
                await client.TrySendMessage(message.Chat.Id, $"Какие Вы мега-шустрые или у Вас маленькая группа. За последние 14 дней, все успели отдежурить. Выбираем случайного: {memberDutyForce.MemberNameDuty}", replyMarkup: new InlineKeyboardMarkup(GenerareKeyboard(memberDutyForce.Id)));
                return;
            }
            case 0 when mainList.Count is 0:
                await client.TrySendMessage(message.Chat.Id, $"Самый редкий случай: Все заболели, все дежурили за последние 14 дней. Выбрать некого :с");
                return;
            case 0 when mainList.Count is not 0:
            {
                var memberDuty = mainList[rnd.Next(0, mainList.Count)];
                await client.TrySendMessage(message.Chat.Id, $"Произошел самый тяжелый случай. Никого нет, все болеют, а те кто есть, уже дежурили, но выбрать все же кого то надо.. Сегодня дежурит: {memberDuty.MemberNameDuty}", replyMarkup: new InlineKeyboardMarkup(GenerareKeyboard(memberDuty.Id)));
                break;
            }
            default:
            {
                var memberDuty = listNonDutiesLast[rnd.Next(0, listNonDutiesLast.Count)];
                await client.TrySendMessage(message.Chat.Id, $"Вжух и пух, выбираю дежурить: {memberDuty.MemberNameDuty}. ", replyMarkup: new InlineKeyboardMarkup(GenerareKeyboard(memberDuty.Id)));
                break;
            }
        }
    }
    
    private IList<IList<InlineKeyboardButton>> GenerareKeyboard(long memberId)
    {
        return new List<IList<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("✅ Принять",
                    $"duty_accept {memberId}"),
                InlineKeyboardButton.WithCallbackData("❌ Отсутствует",
                    $"duty_reject {memberId}")
            },
        };
    }
}