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
        return await ef.History.FirstOrDefaultAsync(x=> x.IdPeer == peerId && x.Date == date);
    }
    
    public async Task<IList<ScheduleHistory>> GetPinnedMessageFromPeerId(long peerId, DateOnly exceptDate)
    {
        return await ef.History.Where(x=> x.IdPeer == peerId)
            .Where(x=> x.IsPinned == true)
            .Where(x=> x.Date != exceptDate)
            .ToListAsync();
    }

    public async Task<ScheduleHistory> CreateScheduleHistory(long peerId, DateOnly date)
    {
        var scheduleHistory = new ScheduleHistory()
        {
            IdPeer = peerId,
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

    public async Task ChangeStatusPinnedMessage(ScheduleHistory message, bool newValue)
    {
        message.IsPinned = newValue;
        await UpdateScheduleHistory(message);
    }
}