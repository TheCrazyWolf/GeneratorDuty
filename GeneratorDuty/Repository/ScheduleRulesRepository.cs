using GeneratorDuty.Database;
using GeneratorDuty.Models.Schedule;
using Microsoft.EntityFrameworkCore;

namespace GeneratorDuty.Repository;

public class ScheduleRulesRepository(DutyContext ef)
{
    public async Task<ScheduleCustomRules?> GetRuleFromDate(DateOnly date)
    {
        return await ef.ScheduleCustomRules.FirstOrDefaultAsync(x=> x.Date == date);
    }

    public async Task CreateRule(ScheduleCustomRules schedule)
    {
        await ef.AddAsync(schedule);
        await ef.SaveChangesAsync();
    }

    public async Task UpdateRule(ScheduleCustomRules schedule)
    {
        ef.Update(schedule);
        await ef.SaveChangesAsync();
    }
}