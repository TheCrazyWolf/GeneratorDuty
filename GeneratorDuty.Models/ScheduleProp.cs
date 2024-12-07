using ClientSamgk.Enums;
using GeneratorDuty.Models.Common;

namespace GeneratorDuty.Models;

public class ScheduleProp : CommonEntity
{
    public long IdPeer { get; set; }
    public ScheduleSearchType SearchType { get; set; }
    public string Value { get; set; } = string.Empty;
    public bool IsAutoSend { get; set; } = false;
    public bool IsAutoExport { get; set; } = false;
    public string LastResult { get; set; } = string.Empty;
    public int Fails { get; set; }
    
    public bool IsRequiredAdminRights { get; set; } = false;
    public bool IsMigrated { get; set; } = false;
}