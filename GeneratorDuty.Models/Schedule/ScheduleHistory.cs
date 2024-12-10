using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using GeneratorDuty.Models.Common;

namespace GeneratorDuty.Models.Schedule;

public class ScheduleHistory : CommonEntity
{
    public DateOnly Date { get; set; }
    public string Result { get; set; } = string.Empty;
    public long? ChatId { get; set; }
    public int? MessageId { get; set; }
}