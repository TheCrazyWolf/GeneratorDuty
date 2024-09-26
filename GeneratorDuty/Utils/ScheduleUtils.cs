using System.Globalization;
using System.Text;
using ClientSamgkOutputResponse.Interfaces.Schedule;

namespace GeneratorDuty.Utils;

public static class ScheduleUtils
{
    public static string GetStringFromRasp(this IResultOutScheduleFromDate scheduleFromDate)
    {
        var msg = new StringBuilder();

        msg.AppendLine($"Расписание на {scheduleFromDate.Date} ({CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(scheduleFromDate.Date.DayOfWeek)})");
        
        foreach (var lesson in scheduleFromDate.Lessons)
        {
            msg.AppendLine($"<blockquote><b>{lesson.DurationStart.ToString()}-{lesson.DurationEnd.ToString()}</b> ({lesson.NumPair}.{lesson.NumLesson})");
            var isAttestation = lesson.SubjectDetails.IsAttestation ? "<b>[Дифф. зачёт]</b> " : string.Empty;
            msg.AppendLine($"{isAttestation} {lesson.SubjectDetails.SubjectName}");
            msg.AppendLine($"{lesson.Identity.First().GetShortName()}");
            msg.AppendLine($"Каб: {lesson.Cabs.FirstOrDefault()?.Adress} • {lesson.EducationGroup.Name}</blockquote>");
        }

        return msg.ToString();
    }
}