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

public class SetCommand(DutyContext ef, ClientSamgkApi clientSamgk) : BaseCommand
{
    public override string Command { get; } = "/set";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;

        message.Text = message.Text.GetReplacedCommandFromDomain().Replace(Command, string.Empty);
        
        if (message.Text.Length > 0)
        {
            int i = message.Text.IndexOf(" ", StringComparison.Ordinal)+1;
            message.Text =message.Text.Substring(i);
        }
        
        var anyGroup = await clientSamgk.Groups.GetGroupAsync(message.Text);
        var anyCab = await clientSamgk.Cabs.GetCabAsync(message.Text);
        var anyTeacher = await clientSamgk.Accounts.GetTeacherAsync(message.Text);

        await GetAndRemoveOlds(message.Chat.Id);

        string value = string.Empty;

        if (anyGroup is not null)
            value = await SetToDb(anyGroup, message.Chat.Id);
        else if (anyTeacher is not null)
            value = await SetToDb(anyTeacher, message.Chat.Id);
        else if (anyCab is not null)
            value = await SetToDb(anyCab, message.Chat.Id);


        if (string.IsNullOrEmpty(value))
            await client.SendTextMessageAsync(message.Chat.Id, "ℹ️ Ничего не нашли");
        else
            await client.SendTextMessageAsync(message.Chat.Id, $"✅ На эту беседу установлено: {value}");
    }

    private async Task<string> SetToDb(IResultOutGroup group, long peerId)
    {
        var prop = new ScheduleProp()
        {
            IdPeer = peerId,
            IsAutoSend = false,
            SearchType = ScheduleSearchType.Group,
            Value = group.Id.ToString()
        };

        await ef.AddAsync(prop);
        await ef.SaveChangesAsync();
        return group.Name;
    }

    private async Task<string> SetToDb(IResultOutIdentity teacher, long peerId)
    {
        var prop = new ScheduleProp()
        {
            IdPeer = peerId,
            IsAutoSend = false,
            SearchType = ScheduleSearchType.Employee,
            Value = teacher.Id.ToString()
        };

        await ef.AddAsync(prop);
        await ef.SaveChangesAsync();
        return teacher.Name;
    }

    private async Task<string> SetToDb(IResultOutCab cab, long peerId)
    {
        var prop = new ScheduleProp()
        {
            IdPeer = peerId,
            IsAutoSend = false,
            SearchType = ScheduleSearchType.Cab,
            Value = cab.Adress
        };

        await ef.AddAsync(prop);
        await ef.SaveChangesAsync();
        return cab.Adress;
    }

    private async Task GetAndRemoveOlds(long peerId)
    {
        foreach (var item in await ef.ScheduleProps
                     .Where(x => x.IdPeer == peerId)
                     .ToListAsync())
        {
            ef.Remove(item);
        }

        await ef.SaveChangesAsync();
    }
    
}