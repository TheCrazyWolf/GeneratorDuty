using GeneratorDuty.Database;

namespace GeneratorDuty.Repository;

public class DutyRepository(DutyContext ef)
{
    public LogMemberPriority LogsMembers = new LogMemberPriority(ef);
    public LogMemberPriority LogsMemberPriority = new LogMemberPriority(ef);
    public MembersRepository Members = new MembersRepository(ef);
    public SchedulePropsRepository ScheduleProps = new SchedulePropsRepository(ef);
}