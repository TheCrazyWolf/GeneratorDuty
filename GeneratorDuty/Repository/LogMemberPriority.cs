using GeneratorDuty.Database;
using GeneratorDuty.Models;
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
    
    public async Task<LogMemberPriority> Create(LogMemberPriority log)
    {
        await ef.AddAsync(log);
        await ef.SaveChangesAsync();
        return log;
    }

    public async Task<LogMemberPriority> Update(LogMemberPriority log)
    {
        ef.Update(log);
        await ef.SaveChangesAsync();
        return log;
    }

    public async Task Remove(LogMemberPriority log)
    {
        ef.Remove(log);
        await ef.SaveChangesAsync();
    }
}