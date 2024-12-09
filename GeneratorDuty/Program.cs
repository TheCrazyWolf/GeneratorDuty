using System.Globalization;
using ClientSamgk;
using GeneratorDuty.BackgroundServices;
using GeneratorDuty.Repository;
using GeneratorDuty.Repository.Database;
using GeneratorDuty.Repository.Duty;
using GeneratorDuty.Services;
using GeneratorDuty.Telegrams;
using GeneratorDuty.Telegrams.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);
CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("ru-RU");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture("ru-RU");

builder.Services.AddHttpClient("telegram_bot_client").RemoveAllLoggers()
    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        var accessTokenTelegram = sp.GetRequiredService<IConfiguration>().GetValue<string>("TelegramBotAccessToken");
        ArgumentNullException.ThrowIfNull(accessTokenTelegram, nameof(accessTokenTelegram));
        TelegramBotClientOptions options = new(accessTokenTelegram);
        return new TelegramBotClient(options, httpClient);
    });

builder.Services.AddScoped<UpdateHandle>();
builder.Services.AddScoped<ReceiverService>();
builder.Services.AddHostedService<PollingService>();
builder.Services.AddTransient<DutyContext>();
builder.Services.AddSingleton<ClientSamgkApi>();
builder.Services.AddSingleton<MemoryExceptionDuty>();
builder.Services.AddTransient<DutyRepository>();
builder.Services.AddHostedService<AutoSendSchedule>();
builder.Services.AddHostedService<AutoSendScheduleNew>();
builder.Services.AddHostedService<AutoSendScheduleExport>();
var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DutyContext>();
    dbContext.Database.Migrate();
}

host.Run();