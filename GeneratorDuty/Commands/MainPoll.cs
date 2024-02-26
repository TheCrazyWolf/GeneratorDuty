using GeneratorDuty.Services;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = Telegram.Bot.Types.File;

namespace GeneratorDuty.Commands;

public class MainPoll : IUpdateHandler
{
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message?.Text == null || update.Type != UpdateType.Message)
        {
            return;
        }
        
        string message = CommandHelper.GetReplacedCommandFromDomain(update.Message?.Text);
        string command = message.Split(' ').First();
        
        switch (command)
        {
            case "/update":
                await UpdateCommand.ExecuteCommand(botClient, update, cancellationToken);
                break;
            case "/get":
                await GetCommand.ExecuteCommand(botClient, update, cancellationToken);
                break;
            case "/clear":
                await ClearCommand.ExecuteCommand(botClient, update, cancellationToken);
                break;
            
        }
        
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;;
    }
}