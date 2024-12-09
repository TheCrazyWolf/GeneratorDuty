using GeneratorDuty.Common;
using GeneratorDuty.Extensions;
using GeneratorDuty.Models;
using GeneratorDuty.Models.Duty;
using GeneratorDuty.Repository;
using GeneratorDuty.Repository.Duty;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GeneratorDuty.Commands.Duty;

public class UpdateCommand(DutyRepository repository) : BaseCommand
{
    public override string Command { get; } = "/update";
    
    private readonly string _usage =
        $"ℹ️ Внесите фамилии через команду /update Фамилия И.О. \n" +
        $"Фамилия И.О.\n" +
        $"и т.д.\n" +
        $"Обязательно каждую фамилию с новой строки";

    public override async Task ExecuteAsync(ITelegramBotClient client, Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || message.From is null) return;
        message.Text = message.Text.GetReplacedCommandFromDomain().Replace(Command, string.Empty);
        
        var prop = await repository.ScheduleProps.GetSchedulePropFromChat(message.Chat.Id);

        if (prop is null)
        {
            await client.TrySendMessage(message.Chat.Id, _usage);
            return;
        }
        
        if (prop.IsRequiredAdminRights && !await client.IsUserAdminInChat(message.From.Id, message.Chat.Id))
        {
            await client.TrySendMessage(message.Chat.Id, "В этом чате данное действие могут выполнять только админы беседы");
            return;
        }

        var membersArray = message.Text.Split('\n');

        if (membersArray.Length is 0 or 1)
        {
            await client.TrySendMessage(message.Chat.Id, _usage);
            return;
        }

        foreach (var member in await repository.Members.GetMembersFromChat(message.Chat.Id))
            await repository.Members.Remove(member);
        
        var countResult = await AddNewDuty(membersArray, message.Chat.Id);
        await client.TrySendMessage(message.Chat.Id, $"✅ Обновили список группы: {countResult}");
    }

    private async Task<int> AddNewDuty(IEnumerable<string> d, long peerId)
    {
        int count = 0;
        foreach (var item in d)
        {
            if(string.IsNullOrEmpty(item) || string.IsNullOrWhiteSpace(item)) continue;

            await repository.Members.Create(new MemberDuty
            {
                IdPeer = peerId,
                MemberNameDuty = item
            });
            count++;
        }
        return count;
    }
}