using ClientSamgk;
using GeneratorDuty.BackgroundServices;
using GeneratorDuty.Database;
using GeneratorDuty.Repository;
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
builder.Services.AddSingleton<ClientSamgkApi>();
builder.Services.AddSingleton<MemoryExceptionDuty>();
builder.Services.AddHostedService<AutoSendSchedule>();
builder.Services.AddHostedService<AutoSendScheduleExport>();
builder.Services.AddTransient<DutyRepository>();

var host = builder.Build();
host.Run();