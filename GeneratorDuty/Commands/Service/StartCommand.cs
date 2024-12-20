using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands.Service;

public class StartCommand() : BaseCommand
{
    public override string Command { get; } = "/start";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        string startMessage = "🤩🥸😛😝 Привет! Мой бот умеет автоматически отправлять расписание в беседу и контроллировать изменения. НУ еще есть рандомайзер дежурных\n\n" +
                              "<b>Установка группы и автоматической рассылки:</b> \n\n" +
                              "1. /set ИС-23-01 или /set ФИО педагога\n" +
                              "2. /auto - включить авто рассылку\n\n" +
                              "<b>Установка списка дежурных:</b> \n" +
                              "1. /update Фамилия И.О. (перенос строки для след. объект обязателен)\n" +
                              "Фамилия И.О. и т.д. \n" +
                              "2. Выбрать дежурного /get\n\n" +
                              "<b>Запрет работу с дежурными и управления настройками бота</b> \n" +
                              "/requireadmin - запретить/разрешить только админам назначать, подтверждать и изменять список дежурных \n\n" +
                              "🍻 Отправлять команду только в ту беседу, где будут закрепляться расписание или дежурные\n\n" +
                              "<b>Другие не особо важные команды:</b> \n\n" +
                              "1. /migrate - переключится на новый/старый движок\n" +
                              "2. /widget - обновляющийся виджет (требуются права админа)\n" +
                              "3. /invalidatecache - очистка кеша\n\n" +
                              "Работает на костылях, так как написано на чистом энтузиазме. Сурсы найти сможете на гитхабе\n\n" +
                              "пожелания и баги сообщать сюда -> @kulagin_alex";
        
        await client.TrySendMessage(message.Chat.Id, startMessage);
    }
}