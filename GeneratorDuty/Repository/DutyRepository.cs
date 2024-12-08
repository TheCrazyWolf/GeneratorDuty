using GeneratorDuty.Database;
using GeneratorDuty.Models.Schedule;

namespace GeneratorDuty.Repository;

public class DutyRepository(DutyContext ef)
{
    public LogsMembers LogsMembers = new LogsMembers(ef);
    public LogMemberPriority LogsMemberPriority = new LogMemberPriority(ef);
    public MembersRepository Members = new MembersRepository(ef);
    public SchedulePropsRepository ScheduleProps = new SchedulePropsRepository(ef);
    public ScheduleHistoryRepository ScheduleHistory = new ScheduleHistoryRepository(ef);
    public ScheduleRulesRepository ScheduleRules = new ScheduleRulesRepository(ef);
}