using System.ComponentModel.DataAnnotations.Schema;
using GeneratorDuty.Models.Common;

namespace GeneratorDuty.Models.Duty;

public class LogDutyMember : CommonEntity
{
    public long? UserId { get; set; }
    [ForeignKey("UserId")] public MemberDuty? Duty { get; set; }
    public DateTime Date { get; set; }
}