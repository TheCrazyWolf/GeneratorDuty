using ClientSamgk;
using ClientSamgk.Enums;
using ClientSamgkOutputResponse.Interfaces.Cabs;
using ClientSamgkOutputResponse.Interfaces.Groups;
using ClientSamgkOutputResponse.Interfaces.Identity;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using GeneratorDuty.Models;
using GeneratorDuty.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands;

public class AutoCommand(DutyContext ef) : BaseCommand
{
    private readonly ClientSamgkApi _clientSamgk = new ClientSamgkApi();
    public override string Command { get; } = "/auto";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        message.Text = message.Text.GetReplacedCommandFromDomain();
        
        var prop = await ef.ScheduleProps.FirstOrDefaultAsync(x=> x.IdPeer == message.Chat.Id);

        if (prop is null)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "ℹ️ На эту беседу нет установленных данных. Используйте /set <фио препода/группа/кабинет>");
            return;
        }
        
        try
        {
            var newParam = Convert.ToBoolean(message.Text.Split(' ')[1]);
            prop.IsAutoSend = newParam;
            ef.Update(prop);
            await ef.SaveChangesAsync();
            await client.SendTextMessageAsync(message.Chat.Id, "✅ Настройки этой беседы обновлены");
        }
        catch (Exception e)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "ℹ️ Не удалось обновить настройки. Usage: /auto true/false");
        }
        
    }
}