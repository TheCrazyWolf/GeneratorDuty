using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Models;
using GeneratorDuty.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GeneratorDuty.Commands;

public class StartCommand() : BaseCommand
{
    public override string Command { get; } = "/start";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        string startMessage = "🤩🥸😛😝 Привет! Мой бот умеет автоматически отправлять расписание в беседу и контроллировать изменения. НУ еще есть рандомайзер дежурных\n\n" +
                              "<b>Установка группы и автоматической рассылки:</b> \n\n" +
                              "1. /set ИС-23-01 или /set ФИО педагога\n" +
                              "2. /auto true - включить авто рассылку, /auto false - выключить\n\n" +
                              "Установка списка дежурны:" +
                              "1. /update Фамилия И.О. (перенос строки для след. объект обязателен)\n" +
                              "Фамилия И.О. и т.д. \n" +
                              "2. Выбрать дежурного /get\n\n" +
                              "🍻 Отправлять команду только в ту беседу, где будут закрепляться расписание или дежурные\n\n" +
                              "Работает на костылях, так как написано на чистом энтузиазме. Сурсы найти сможете на гитхабе";
        
        try
        {
            await client.SendTextMessageAsync(message.Chat.Id, startMessage, parseMode: ParseMode.Html);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}