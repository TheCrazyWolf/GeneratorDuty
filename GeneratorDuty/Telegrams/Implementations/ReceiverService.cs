using GeneratorDuty.Telegrams.Abstractions;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace GeneratorDuty.Telegrams.Implementations;

public class ReceiverService(ITelegramBotClient botClient, UpdateHandle updateHandler, ILogger<ReceiverServiceBase<UpdateHandle>> logger)
    : ReceiverServiceBase<UpdateHandle>(botClient, updateHandler, logger);