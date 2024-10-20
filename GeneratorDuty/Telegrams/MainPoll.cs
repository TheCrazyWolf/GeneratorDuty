using ClientSamgk;
using GeneratorDuty.CallBackKeyboards;
using GeneratorDuty.Commands;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GeneratorDuty.Telegrams;

public class MainPoll(DutyContext ef, ClientSamgkApi clientSamgk) : IUpdateHandler
{
    private readonly IReadOnlyCollection<BaseCommand> _commands = new List<BaseCommand>()
    {
        new SetCommand(ef, clientSamgk),
        new GetCommand(ef),
        new UpdateCommand(ef),
        new TodayCommand(ef, clientSamgk),
        new TomorrowCommand(ef, clientSamgk),
        new AutoCommand(ef),
        new ExportCommand(clientSamgk),
        new SAutoCommand(ef),
        new StartCommand(),
        new SayCommand(ef)
    };

    private readonly IReadOnlyCollection<CallQuery> _callQueries = new List<CallQuery>()
    {
        new DutyAccept(ef), new DutyReject(ef)
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

        if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
        {
            foreach (var item in _callQueries)
            {
                if(!item.Contains(update.CallbackQuery)) continue;
                item.Execute(botClient, update.CallbackQuery);
            }
        }
    }
    

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;;
    }
}