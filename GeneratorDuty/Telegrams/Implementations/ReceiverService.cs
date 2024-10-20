using GeneratorDuty.Telegrams.Abstractions;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace GeneratorDuty.Telegrams.Implementations;

public class ReceiverService(ITelegramBotClient botClient, MainPoll updateHandler, ILogger<ReceiverServiceBase<MainPoll>> logger)
    : ReceiverServiceBase<MainPoll>(botClient, updateHandler, logger);