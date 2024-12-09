using System.ComponentModel.DataAnnotations.Schema;
using GeneratorDuty.Models.Common;

namespace GeneratorDuty.Models;

public class LogDutyMemberPriority : CommonEntity
{
    public long? UserId { get; set; }
    [ForeignKey("UserId")] public MemberDuty? Duty { get; set; }

}