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
    
    private readonly Random _rnd = new Random();
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
        
        var lostedMembers = await GetLostsFromChat(message.Chat.Id);

        foreach (var member in lostedMembers)
        {
            var foundInHistory = await FoundInHistory(member.Duty!);
            if(foundInHistory is not null) continue;
            
            await client.SendTextMessageAsync(message.Chat.Id, $"✅ Я помню, как кое-то убежал от меня. Время настало. Дежурит: {member.Duty?.MemberNameDuty}", replyMarkup: new InlineKeyboardMarkup(GenerateKeyboardForNotify(member.Id)));
            return;
        }
        
        /* ------ */
        
        /* Не пришедшие в прошлый раз */
        
        var listNonDutiesLast = mainList.ToList();
        
        foreach (var member in mainList)
        {
            var item = await FoundInHistory(member);
            if(item is null) continue;
            listNonDutiesLast.Remove(member);
        }
        
        if (listNonDutiesLast.Count is 0 && mainList.Count is not 0)
        {
            var memberDutyForce = mainList[_rnd.Next(0, mainList.Count)];
            await client.SendTextMessageAsync(message.Chat.Id, $"ℹ️ Что? Якобы все отдежурили за последние 7 дней?)) Ха-ха, никак не помешает мне выбрать: {memberDutyForce.MemberNameDuty}", replyMarkup: new InlineKeyboardMarkup(GenerateKeyboardForNotify(memberDutyForce.Id)));
            return;
        }
        
        var memberDuty = listNonDutiesLast[_rnd.Next(0, listNonDutiesLast.Count)];
        
        await client.SendTextMessageAsync(message.Chat.Id, $"✅ Сегодня дежурит: {memberDuty.MemberNameDuty}", replyMarkup: new InlineKeyboardMarkup(GenerateKeyboardForNotify(memberDuty.Id)));
    }

    // Выгрузка тех, кто попал на дежурство, но отдежурил
    private async Task<List<LogDutyMemberLost>> GetLostsFromChat(long chatId)
    { 
        var list = await ef.LogDutyMemberLosts
            .Include(x => x.Duty)
            .Where(x => x.Duty!.IdPeer == chatId
                        && x.Date >= DateTime.Now.AddDays(-14))
            .ToListAsync();

        foreach (var found in Cache.GetFromChats(chatId)
                     .Select(item => list.FirstOrDefault(x => x.Id == item.Id))
                     .OfType<LogDutyMemberLost>())
        {
            list.Remove(found);
        }

        return list;
    }
    
    // Поиск в истории дежурства за последние 7 дней
    private async Task<LogDutyMember?> FoundInHistory(MemberDuty duty)
    {
        return await ef.LogDutyMembers
            .FirstOrDefaultAsync(x => x.UserId == duty.Id
                                      && x.Date >= DateTime.Now.AddDays(-14));
    }
    
    private IList<IList<InlineKeyboardButton>> GenerateKeyboardForNotify(long dutyId)
    {
        return new List<IList<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("🖌 Отдежурил",
                    $"duty_accept {dutyId}"),
                InlineKeyboardButton.WithCallbackData("❌ Его нет",
                    $"duty_reject {dutyId}")
            },
        };
    }
}