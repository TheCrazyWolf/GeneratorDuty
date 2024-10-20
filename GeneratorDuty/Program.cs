using ClientSamgk;
using GeneratorDuty.BackgroundServices;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Services;
using GeneratorDuty.Telegrams;
using Telegram.Bot;

string token = args.FirstOrDefault() ?? string.Empty;
        
if (string.IsNullOrWhiteSpace(token))
    throw new Exception("Не указан токен");

ITelegramBotClient botClient = new TelegramBotClient(token);

ClientSamgkApi samgkApi = new ClientSamgkApi();

IReadOnlyCollection<BaseTask> tasks = new List<BaseTask>
{
    new AutoSendSchedule(botClient, new DutyContext(), samgkApi),
    new AutoSendScheduleExport(botClient, new DutyContext(), samgkApi)
};

var me = await botClient.GetMeAsync();
CommandStingUtils.Me = me.Username ?? string.Empty;

foreach (var task in tasks)
    _ = task.RunAsync();

await botClient.ReceiveAsync(new MainPoll(new DutyContext(), samgkApi));

await Task.Delay(-1);