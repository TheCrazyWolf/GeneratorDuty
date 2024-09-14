using GeneratorDuty.BackgroundServices;
using GeneratorDuty.Common;

namespace GeneratorDuty;
using Database;
using Services;
using Telegrams;
using Telegram.Bot;

static class Program
{
    private static readonly string _token = string.Empty;
    private static readonly ITelegramBotClient _botClient = new TelegramBotClient(_token);
    private static readonly DutyContext _ef = new DutyContext();
    private static IReadOnlyCollection<BaseTask> _tasks = new List<BaseTask>()
    {
        new AutoSendSchedule(_botClient, _ef)
    };
    
    static async Task Main(string[] args)
    {
        // See https://aka.ms/new-console-template for more information
        var me = await _botClient.GetMeAsync();
        CommandStingUtils.Me = me.Username ?? string.Empty;
        await _botClient.ReceiveAsync(new MainPoll(new DutyContext()));

        foreach (var task in _tasks)
            _ = task.RunAsync();
        
        await Task.Delay(-1);
    }
}