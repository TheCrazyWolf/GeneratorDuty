using Microsoft.Extensions.Hosting;
using Timer =  System.Timers.Timer;

namespace GeneratorDuty.Common;

public class BackgroundServiceBase : BackgroundService
{
    protected readonly Timer Timer = new Timer
    {
        #if debug
        Interval = 10000,
        #else
        Interval = 300000, // 300000
        #endif
    };
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
    
    protected bool CanWorkSerivce(DateTime nowTime)
    {
        #if debug
        return true;
        #else
        return nowTime.Hour switch
        {
            >= 19 or <= 7 => false,
            _ => true
        };
        #endif
    }
}