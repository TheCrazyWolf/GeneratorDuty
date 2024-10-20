using GeneratorDuty.Database;
using GeneratorDuty.Models;
using Microsoft.EntityFrameworkCore;

namespace GeneratorDuty.Repository;

public class SchedulePropsRepository(DutyContext ef)
{
    public async Task<ScheduleProp?> GetScheduleProp(long id)
    {
        return await ef.ScheduleProps.FirstOrDefaultAsync(x=> x.Id == id);
    }

    public async Task<IEnumerable<ScheduleProp>> GetSchedulePropsFromChat(long chatId)
    {
        return await ef.ScheduleProps.Where(x=> x.IdPeer == chatId).ToListAsync();
    }
    
    public async Task<IEnumerable<ScheduleProp>> GetSchedulePropsFromAutoSend(bool isConfiguredAutoSend)
    {
        return await ef.ScheduleProps.Where(x=> x.IsAutoSend == isConfiguredAutoSend).ToListAsync();
    }
    
    public async Task<IEnumerable<ScheduleProp>> GetSchedulePropsFromAutoExport(bool isConfiguredAutoExport)
    {
        return await ef.ScheduleProps.Where(x=> x.IsAutoExport == isConfiguredAutoExport).ToListAsync();
    }
    
    public async Task<ScheduleProp> Create(ScheduleProp scheduleProp)
    {
        await ef.AddAsync(scheduleProp);
        await ef.SaveChangesAsync();
        return scheduleProp;
    }

    public async Task<ScheduleProp> Update(ScheduleProp scheduleProp)
    {
        ef.Update(scheduleProp);
        await ef.SaveChangesAsync();
        return scheduleProp;
    }

    public async Task Remove(ScheduleProp scheduleProp)
    {
        ef.Remove(scheduleProp);
        await ef.SaveChangesAsync();
    }
}