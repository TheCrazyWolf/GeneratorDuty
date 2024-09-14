using GeneratorDuty.BackgroundServices;
using GeneratorDuty.Common;

namespace GeneratorDuty;

using Database;
using Services;
using Telegrams;
using Telegram.Bot;

static class Program
{
    private static string _token = "";
    private static  ITelegramBotClient _botClient = default!;
    private static readonly DutyContext _ef = new DutyContext();

    private static IReadOnlyCollection<BaseTask> _tasks = new List<BaseTask>()
    {
        new AutoSendSchedule(_botClient, _ef)
    };

    static async Task Main(string[] args)
    {
        _token = args.FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(_token))
            throw new Exception("Не указан токен");

        _botClient = new TelegramBotClient(_token);
        Console.WriteLine(_token);

        // See https://aka.ms/new-console-template for more information
        var me = await _botClient.GetMeAsync();
        CommandStingUtils.Me = me.Username ?? string.Empty;
        foreach (var task in _tasks)
            _ = task.RunAsync();

        await _botClient.ReceiveAsync(new MainPoll(_ef));

        await Task.Delay(-1);
    }
}