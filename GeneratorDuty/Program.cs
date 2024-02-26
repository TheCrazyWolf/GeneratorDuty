// See https://aka.ms/new-console-template for more information

using GeneratorDuty.Commands;
using Telegram.Bot;

string token = "6917225080:AAF9MX2V4HX2rKVbJX7O3DW3xIhsyvcUun4";

var botClient = new TelegramBotClient(token);

await botClient.ReceiveAsync(new MainPoll());

await Task.Delay(-1);
