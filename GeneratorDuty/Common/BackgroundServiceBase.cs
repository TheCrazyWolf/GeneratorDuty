using Microsoft.Extensions.Hosting;
using Timer =  System.Timers.Timer;

namespace GeneratorDuty.Common;

public class BackgroundServiceBase : BackgroundService
{
    protected readonly Timer Timer = new Timer
    {
        Interval = 300000, // 300000
    };
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
    
    protected bool CanWorkSerivce(DateTime nowTime)
    {
        return nowTime.Hour switch
        {
            >= 19 or <= 7 => false,
            _ => true
        };
    }
}