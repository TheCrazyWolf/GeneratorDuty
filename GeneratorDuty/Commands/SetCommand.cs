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

public class SetCommand(DutyContext ef) : BaseCommand
{
    private readonly ClientSamgkApi _clientSamgk = new ClientSamgkApi();
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
        
        var anyGroup = await _clientSamgk.Groups.GetGroupAsync(message.Text);
        var anyCab = await _clientSamgk.Cabs.GetCabAsync(message.Text);
        var anyTeacher = await _clientSamgk.Accounts.GetTeacherAsync(message.Text);

        await GetAndRemoveOlds(message.From.Id);

        string value = string.Empty;

        if (anyGroup is not null)
            value = await SetToDb(anyGroup, message.From.Id);
        else if (anyTeacher is not null)
            value = await SetToDb(anyTeacher, message.From.Id);
        else if (anyCab is not null)
            value = await SetToDb(anyCab, message.From.Id);


        if (string.IsNullOrEmpty(value))
            await client.SendTextMessageAsync(message.From.Id, "ℹ️ Ничего не нашли");
        else
            await client.SendTextMessageAsync(message.From.Id, $"✅ На эту беседу установлено: {value}");
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

    private async Task AddNewDuty(IEnumerable<string> d, long peerId)
    {
        foreach (var item in d)
        {
            if (string.IsNullOrEmpty(item)) continue;

            await ef.AddAsync(new MemberDuty
            {
                IdPeer = peerId,
                MemberNameDuty = item
            });
        }
        await ef.SaveChangesAsync();
    }
}