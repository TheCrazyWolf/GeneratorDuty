using GeneratorDuty.Database;
using GeneratorDuty.Models;
using GeneratorDuty.Models.Duty;
using Microsoft.EntityFrameworkCore;

namespace GeneratorDuty.Repository;

public class LogMemberPriority(DutyContext ef)
{
    public async Task<LogDutyMemberPriority?> GetLog(long id)
    {
        return await ef.LogDutyMemberPriorities.FirstOrDefaultAsync(x=> x.Id == id);
    }
    
    public async Task<List<LogDutyMemberPriority>> GetLogsByIdMember(long id)
    {
        return await ef.LogDutyMemberPriorities.Where(x=> x.UserId == id).ToListAsync();
    }
    
    public async Task<List<LogDutyMemberPriority>> GetLogsFromChatId(long id)
    {
        return await ef.LogDutyMemberPriorities
            .Include(x=> x.Duty).Where(x=> x.Duty!.IdPeer == id).ToListAsync();
    }
    
    public async Task<LogDutyMemberPriority> Create(LogDutyMemberPriority log)
    {
        await ef.AddAsync(log);
        await ef.SaveChangesAsync();
        return log;
    }

    public async Task<LogDutyMemberPriority> Update(LogDutyMemberPriority log)
    {
        ef.Update(log);
        await ef.SaveChangesAsync();
        return log;
    }

    public async Task Remove(LogDutyMemberPriority log)
    {
        ef.Remove(log);
        await ef.SaveChangesAsync();
    }
}