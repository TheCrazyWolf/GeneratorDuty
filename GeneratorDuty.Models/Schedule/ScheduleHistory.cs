using System.ComponentModel.DataAnnotations.Schema;
using GeneratorDuty.Models.Common;

namespace GeneratorDuty.Models.Schedule;

public class ScheduleHistory : CommonEntity
{
    public long? IdPeer { get; set; }
    [ForeignKey("")] public ScheduleProp? PropPeer { get; set; }
    public DateOnly Date { get; set; }
    public string Result { get; set; } = string.Empty;
}