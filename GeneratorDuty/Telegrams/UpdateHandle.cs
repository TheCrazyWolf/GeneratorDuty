using ClientSamgk;
using GeneratorDuty.CallBackKeyboards;
using GeneratorDuty.CallBackKeyboards.Duty;
using GeneratorDuty.CallBackKeyboards.Schedule;
using GeneratorDuty.Commands;
using GeneratorDuty.Commands.Duty;
using GeneratorDuty.Commands.Schedule;
using GeneratorDuty.Commands.Service;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Extensions;
using GeneratorDuty.Repository;
using GeneratorDuty.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GeneratorDuty.Telegrams;

public class UpdateHandle(DutyRepository ef, ClientSamgkApi clientSamgk, MemoryExceptionDuty cache, ILogger<UpdateHandle> logger) : IUpdateHandler
{
    private readonly IReadOnlyCollection<BaseCommand> _commands = new List<BaseCommand>()
    {
        new SetCommand(ef, clientSamgk),
        new GetCommand(ef,cache),
        new UpdateCommand(ef),
        new OpenCommand(ef, clientSamgk),
        new AutoCommand(ef),
        new ExportCommand(clientSamgk),
        new SAutoCommand(ef),
        new StartCommand(),
        new SayCommand(ef),
        new HistoryCommand(ef),
        new ForceCommand(ef),
        new RestrictionPeer(ef)
    };

    private readonly IReadOnlyCollection<CallQuery> _callQueries = new List<CallQuery>()
    {
        new DutyAccept(ef), new DutyReject(ef, cache), new DutyForce(ef), new DutyForceCancel(),
        new ScheduleKeyboard(clientSamgk)
    };
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        switch (update.Type)
        {
            case UpdateType.Message when update.Message?.From != null:
            {
                foreach (var command in _commands)
                {
                    if(!command.Contains(update.Message))
                        continue;
                    
                    logger.LogInformation($"Обработка команды: {command.Command}. от: ID {update.Message.From.Id} в чате: {update.Message.Chat.Id}");
                    await command.ExecuteAsync(botClient, update.Message);
                }
                logger.LogInformation($"Сообщение: {update.Message.MessageId}. от: ID {update.Message.From.Id} в чате: {update.Message.Chat.Id} c текстом: {update.Message.Text}");
                break;
            }
            
            case UpdateType.CallbackQuery when update.CallbackQuery?.Message != null:
            {
                foreach (var item in _callQueries)
                {
                    if(!item.Contains(update.CallbackQuery)) continue;
                    logger.LogInformation($"Обработка CallBackQuery: {item.Name}. от: ID {update.CallbackQuery.From.Id} в чате: {update.CallbackQuery.Message.Chat.Id}");
                    item.Execute(botClient, update.CallbackQuery);
                }

                break;
            }
            case UpdateType.Unknown:
                break;
            case UpdateType.InlineQuery:
                break;
            case UpdateType.ChosenInlineResult:
                break;
            case UpdateType.EditedMessage:
                break;
            case UpdateType.ChannelPost:
                break;
            case UpdateType.EditedChannelPost:
                break;
            case UpdateType.ShippingQuery:
                break;
            case UpdateType.PreCheckoutQuery:
                break;
            case UpdateType.Poll:
                break;
            case UpdateType.PollAnswer:
                break;
            case UpdateType.MyChatMember:
                break;
            case UpdateType.ChatMember:
                break;
            case UpdateType.ChatJoinRequest:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;;
    }
}