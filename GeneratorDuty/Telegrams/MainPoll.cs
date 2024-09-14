using GeneratorDuty.Commands;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GeneratorDuty.Telegrams;

public class MainPoll(DutyContext ef) : IUpdateHandler
{
    private readonly IReadOnlyCollection<BaseCommand> _commands = new List<BaseCommand>()
    {
        new SetCommand(ef),
        new GetCommand(ef),
        new UpdateCommand(ef),
        new TodayCommand(ef),
        new TomorrowCommand(ef),
        new AutoCommand(ef)
    };
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message != null)
        {
            foreach (var command in _commands)
            {
                if(!command.Contains(update.Message))
                    continue;

                await command.ExecuteAsync(botClient, update.Message);
            }
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;;
    }
}