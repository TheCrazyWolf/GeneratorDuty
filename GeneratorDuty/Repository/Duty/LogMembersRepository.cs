using GeneratorDuty.Models.Duty;
using GeneratorDuty.Repository.Database;
using Microsoft.EntityFrameworkCore;

namespace GeneratorDuty.Repository.Duty;

public class LogsMembers(DutyContext ef)
{
    public async Task<LogDutyMember?> GetLog(long id)
    {
        return await ef.LogDutyMembers.FirstOrDefaultAsync(x=> x.Id == id);
    }
    
    public async Task<List<LogDutyMember>> GetLogsByIdMember(long id)
    {
        return await ef.LogDutyMembers.Where(x=> x.UserId == id).ToListAsync();
    }

    public async Task<LogDutyMember?> FoundInHistory(long memberId, int days)
    {
        return await ef.LogDutyMembers
            .FirstOrDefaultAsync(x => x.UserId == memberId
                                      && DateTime.Now.AddDays(-days) <= x.Date);
    }
    
    public async Task<LogDutyMember> Create(LogDutyMember log)
    {
        await ef.AddAsync(log);
        await ef.SaveChangesAsync();
        return log;
    }

    public async Task<LogDutyMember> Update(LogDutyMember log)
    {
        ef.Update(log);
        await ef.SaveChangesAsync();
        return log;
    }

    public async Task Remove(LogDutyMember log)
    {
        ef.Remove(log);
        await ef.SaveChangesAsync();
    }

    public async Task<LogDutyMember?> GetLastLog(MemberDuty item)
    {
        return await ef.LogDutyMembers
            .Where(x => x.UserId == item.Id)
            .OrderBy(x=> x.Date).LastOrDefaultAsync();
    }
}