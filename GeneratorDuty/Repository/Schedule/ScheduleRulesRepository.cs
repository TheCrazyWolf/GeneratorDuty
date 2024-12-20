﻿using ClientSamgkOutputResponse.Enums;
using GeneratorDuty.Models.Schedule;
using GeneratorDuty.Repository.Database;
using Microsoft.EntityFrameworkCore;

namespace GeneratorDuty.Repository.Schedule;

public class ScheduleRulesRepository(DutyContext ef)
{
    public async Task<ScheduleCustomRules> GetRuleFromDateOrDefault(DateOnly date)
    {
        return await ef.ScheduleCustomRules.FirstOrDefaultAsync(x=> x.Date == date) ?? new ScheduleCustomRules()
        {
            Date = date,
            CallType = ScheduleCallType.Standart,
            ShowRussianHorizont = true,
            ShowImportantLesson = true
        };
    }
    
    public async Task<ScheduleCustomRules?> GetRuleFromDate(DateOnly date)
    {
        return await ef.ScheduleCustomRules.FirstOrDefaultAsync(x=> x.Date == date);
    }

    public async Task CreateRule(ScheduleCustomRules schedule)
    {
        await ef.AddAsync(schedule);
        await ef.SaveChangesAsync();
    }

    public async Task UpdateRule(ScheduleCustomRules schedule, ScheduleCallType callType,
        bool showImportantLesson, bool showRussianHorizont)
    {
        schedule.CallType = callType;
        schedule.ShowImportantLesson = showImportantLesson;
        schedule.ShowRussianHorizont = showRussianHorizont;
        ef.Update(schedule);
        await ef.SaveChangesAsync();
    }
}