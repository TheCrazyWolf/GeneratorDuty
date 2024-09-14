using GeneratorDuty.Common;
using GeneratorDuty.Database;
using Telegram.Bot;

namespace GeneratorDuty.BackgroundServices;

public class AutoSendSchedule(ITelegramBotClient client, DutyContext ef) : BaseTask
{
    public override Task RunAsync()
    {
        throw new NotImplementedException();
    }

    private async Task WorkSerivce()
    {
        while (true)
        {
            await Task.Delay(1000);
        }
        
    }
}