using ClientSamgkOutputResponse.Enums;
using GeneratorDuty.Models.Common;

namespace GeneratorDuty.Models.Schedule;

public class ScheduleCustomRules : CommonEntity
{
    public DateOnly Date { get; set; }
    public ScheduleCallType CallType { get; set; }
    public bool ShowImportantLesson { get; set; }
    public bool ShowRussianHorizont { get; set; }
    
    public ScheduleCustomRules() {}

    public ScheduleCustomRules(DateOnly date, ScheduleCallType callType, bool showImportantLesson,
        bool showRussianHorizont)
    {
        Date = date;
        CallType = callType;
        ShowImportantLesson = showImportantLesson;
        ShowRussianHorizont = showRussianHorizont;
    }
}