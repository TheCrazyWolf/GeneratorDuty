using ClientSamgk.Enums;
using GeneratorDuty.Models.Common;

namespace GeneratorDuty.Models;

public class ScheduleProp : CommonEntity
{
    public long IdPeer { get; set; }
    ScheduleSearchType SearchType { get; set; }
    public string Value { get; set; } = string.Empty;
}