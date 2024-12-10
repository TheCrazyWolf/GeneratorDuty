using GeneratorDuty.Models.Schedule;
using GeneratorDuty.Repository.Database;
using Microsoft.EntityFrameworkCore;

namespace GeneratorDuty.Repository.Schedule;

public class ScheduleHistoryRepository(DutyContext ef)
{
    public async Task<ScheduleHistory> CreateOrGetScheduleHistory(long peerId, DateOnly date)
    {
        var scheduleHistory = await GetScheduleHistory(peerId, date);
        return scheduleHistory ?? await CreateScheduleHistory(peerId, date);
    }
    
    public async Task<ScheduleHistory?> GetScheduleHistory(long peerId, DateOnly date)
    {
        return await ef.History.FirstOrDefaultAsync(x=> x.ChatId == peerId && x.Date == date);
    }
    
    public async Task<ScheduleHistory> CreateScheduleHistory(long peerId, DateOnly date)
    {
        var scheduleHistory = new ScheduleHistory()
        {
            ChatId = peerId,
            Date = date
        };
        
        await ef.AddAsync(scheduleHistory);
        await ef.SaveChangesAsync();
        return scheduleHistory;
    }
    
    public async Task<ScheduleHistory> CreateScheduleHistory(ScheduleHistory scheduleHistory)
    {
        await ef.AddAsync(scheduleHistory);
        await ef.SaveChangesAsync();
        return scheduleHistory;
    }

    public async Task UpdateScheduleHistory(ScheduleHistory scheduleHistory)
    {
        ef.Update(scheduleHistory);
        await ef.SaveChangesAsync();
    }

    public async Task<int> InvalidateLocalCache(long chatId)
    {
        var cache = await ef.History.Where(x=> x.ChatId == chatId).ToListAsync();
        var count = cache.Count;
        foreach (var item in cache) ef.Remove(item);
        await ef.SaveChangesAsync();
        return count;
    }

    public async Task<int> InvalidateLocalCacheGlobal()
    {
        var toLastDay = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);
        var cache = await ef.History.Where(x => x.Date <= toLastDay).ToListAsync();
        var count = cache.Count;
        foreach (var item in cache) ef.Remove(item);
        await ef.SaveChangesAsync();
        return count;
    }
}