using GeneratorDuty.Telegrams.Abstractions;
using Microsoft.Extensions.Logging;

namespace GeneratorDuty.Telegrams.Implementations;

public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
    : PollingServiceBase<ReceiverService>(serviceProvider, logger);