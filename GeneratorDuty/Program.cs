// See https://aka.ms/new-console-template for more information

using GeneratorDuty.Database;
using GeneratorDuty.Services;
using GeneratorDuty.Telegrams;
using Telegram.Bot;

string token = "6917225080:AAF9MX2V4HX2rKVbJX7O3DW3xIhsyvcUun4";

var botClient = new TelegramBotClient(token);

var me = await botClient.GetMeAsync();
CommandStingUtils.Me = me.Username ?? string.Empty;

await botClient.ReceiveAsync(new MainPoll(new DutyContext()));

await Task.Delay(-1);
