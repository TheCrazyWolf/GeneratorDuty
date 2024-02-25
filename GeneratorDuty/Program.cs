// See https://aka.ms/new-console-template for more information

using GeneratorDuty.Commands;
using Telegram.Bot;

string token = "6917225080:AAEQ3CiIryqdOCjRI30p6hKiJ1VK0iUNq70";

var botClient = new TelegramBotClient(token);

await botClient.ReceiveAsync(new MainPoll());

await Task.Delay(-1);
