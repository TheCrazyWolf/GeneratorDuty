using ClientSamgk;
using GeneratorDuty.BackgroundServices;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Services;
using GeneratorDuty.Telegrams;
using GeneratorDuty.Telegrams.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;



var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("telegram_bot_client").RemoveAllLoggers()
    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        TelegramBotClientOptions options = new(args.First());
        return new TelegramBotClient(options, httpClient);
    });

builder.Services.AddScoped<UpdateHandle>();
builder.Services.AddScoped<ReceiverService>();
builder.Services.AddHostedService<PollingService>();
builder.Services.AddDbContext<DutyContext>();

var host = builder.Build();
host.Run();

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

await botClient.ReceiveAsync(new UpdateHandle(new DutyContext(), samgkApi));

await Task.Delay(-1);