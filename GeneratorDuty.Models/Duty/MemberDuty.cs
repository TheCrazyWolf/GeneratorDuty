﻿using GeneratorDuty.Models.Common;

namespace GeneratorDuty.Models.Duty;

public class MemberDuty : CommonEntity
{
    public long IdPeer { get; set; }
    public string MemberNameDuty { get; set; } = string.Empty;
}