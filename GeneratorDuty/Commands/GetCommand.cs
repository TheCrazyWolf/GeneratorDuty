using GeneratorDuty.Cache;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.Commands;

public class GetCommand(DutyContext ef) : BaseCommand
{
    public static MemoryExceptionDuty Cache { get; set; } = new ();
    
    public override string Command { get; } = "/get";
    
    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;
        
        var mainList = await ef.MemberDuties
            .Where(x => x.IdPeer == message.Chat.Id)
            .ToListAsync();

        if (mainList.Count is 0)
        {
            await client.SendTextMessageAsync(message.Chat.Id, $"ℹ️ В это беседе не настроены списки дежурных, юзай /update и укажи список группы разделяя переносом строки ФИО");
            return;
        }
        
        /* Исключаем из списка тех, кого сейчас нет */

        foreach (var found in Cache.GetFromChats(message.Chat.Id)
                     .Select(item => mainList
                         .FirstOrDefault(x => x.Id == item.Id)).OfType<MemberDuty>())
        {
            mainList.Remove(found);
        }
        
        /* ------ */
        
        /* Не пришедшие в прошлый раз */
        
        var memberPriorities = await GetPriorityMembersFromChat(message.Chat.Id);

        foreach (var member in memberPriorities)
        {
            var foundInHistory = await FoundInHistory(member.Duty!);
            if (foundInHistory is not null) continue;
            
            await client.SendTextMessageAsync(message.Chat.Id, $"Я надеюсь, сегодня то {member.Duty?.MemberNameDuty} пришел? За тобой должок остался", replyMarkup: new InlineKeyboardMarkup(GenerateKeyboardForNotify(member.Duty!.Id)));
            return;
        }
        
        /* ------ */
        
        /* выборка из тех кто давно не дежурил з */
        
        var listNonDutiesLast = mainList.ToList();
        
        foreach (var member in mainList)
        {
            var item = await FoundInHistory(member);
            if(item is null) continue;
            listNonDutiesLast.Remove(member);
        }
        
        Random rnd = new Random();
        
        if (listNonDutiesLast.Count is 0 && mainList.Count is not 0)
        {
            var memberDutyForce = mainList[rnd.Next(0, mainList.Count)];
            await client.SendTextMessageAsync(message.Chat.Id, $"Какие Вы мега-шустрые или у Вас маленькая группа. За последние 14 дней, все успели отдежурить. Выбираем случайного: {memberDutyForce.MemberNameDuty}", replyMarkup: new InlineKeyboardMarkup(GenerateKeyboardForNotify(memberDutyForce.Id)));
            return;
        }

        if (listNonDutiesLast.Count is 0 && mainList.Count is 0)
        {
            await client.SendTextMessageAsync(message.Chat.Id, $"Самый редкий случай: Все заболели, все дежурили за последние 14 дней. Выбрать некого :с");
            return;
        }
        
        if (listNonDutiesLast.Count is 0 && mainList.Count is not 0)
        {
            var memberDuty = mainList[rnd.Next(0, mainList.Count)];
            await client.SendTextMessageAsync(message.Chat.Id, $"Произошел самый тяжелый случай. Никого нет, все болеют, а те кто есть, уже дежурили, но выбрать все же кого то надо.. Сегодня дежурит: {memberDuty.MemberNameDuty}", replyMarkup: new InlineKeyboardMarkup(GenerateKeyboardForNotify(memberDuty.Id)));
        }
        else
        {
            var memberDuty = listNonDutiesLast[rnd.Next(0, listNonDutiesLast.Count)];
            await client.SendTextMessageAsync(message.Chat.Id, $"Вжух и пух, выбираю дежурить: {memberDuty.MemberNameDuty}. ", replyMarkup: new InlineKeyboardMarkup(GenerateKeyboardForNotify(memberDuty.Id)));
        }
        
    }

    // Выгрузка тех, кто попал на дежурство, но отдежурил
    private async Task<List<LogDutyMemberPriority>> GetPriorityMembersFromChat(long chatId)
    { 
        var list = await ef.LogDutyMemberPriorities
            .Include(x => x.Duty)
            .Where(x => x.Duty!.IdPeer == chatId)
            .ToListAsync();
        
        foreach (var member in Cache.GetFromChats(chatId).ToList())
        {
            foreach (var log in list.Where(x => x.Duty!.Id == member.Id).ToList())
            {
                list.Remove(log);
            }
        }
        
        return list;
    }
    
    // Поиск в истории дежурства за последние 14 дней
    private async Task<LogDutyMember?> FoundInHistory(MemberDuty duty)
    {
        return await ef.LogDutyMembers
            .FirstOrDefaultAsync(x => x.UserId == duty.Id
                                      && DateTime.Now.AddDays(-14) <= x.Date);
    }
    
    private IList<IList<InlineKeyboardButton>> GenerateKeyboardForNotify(long dutyId)
    {
        return new List<IList<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("✅ Принять",
                    $"duty_accept {dutyId}"),
                InlineKeyboardButton.WithCallbackData("❌ Отсутствует",
                    $"duty_reject {dutyId}")
            },
        };
    }
}