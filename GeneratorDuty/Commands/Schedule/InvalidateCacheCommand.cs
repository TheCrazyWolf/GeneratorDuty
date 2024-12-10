using ClientSamgk;
using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models.Schedule;
using GeneratorDuty.Repository;
using GeneratorDuty.Repository.Duty;
using GeneratorDuty.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.Commands.Schedule;

public class InvalidateCacheCommand(DutyRepository repository, ClientSamgkApi clientSamgkApi) : BaseCommand
{
    public override string Command { get; } = "/invalidatecache";
    // options - clear
    // global - глобальный кеш
    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        message.Text = message.Text.GetReplacedCommandFromDomain().Replace(Command, string.Empty);

        if (message.Text.Contains("global"))
        {
            clientSamgkApi.Cache.Clear();
            var count = await repository.ScheduleHistory.InvalidateLocalCacheGlobal();
            await client.TrySendMessage(message.Chat.Id, $"ℹ️ Удалено: {count} элементов из кеша.");
            return;
        }

        if (message.Text.Contains("confirm"))
        {
            var count = await repository.ScheduleHistory.InvalidateLocalCache(message.Chat.Id);
            await clientSamgkApi.Cache.ClearIfOutDateAsync();
            await client.TrySendMessage(message.Chat.Id, $"ℹ️ Удалено: {count} элементов из кеша.");
            return;
        }
        
        await client.TrySendMessage(message.Chat.Id, $"ℹ️ Очистка кеша\n\nЕсли возникли проблемы с отображением расписанием, " +
                                                     $"Вы можете очистить кеш расписания для этой беседы. \n\nОтправьте команду, если согласны - /invalidatecache confirm");
        
        
    }
}