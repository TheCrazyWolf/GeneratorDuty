using GeneratorDuty.Models.Common;

namespace GeneratorDuty.Models.Schedule;

public class MessageWidget : CommonEntity
{
    public long ChatId { get; set; }
    public int MessageId { get; set; }
}