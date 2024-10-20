using System.Globalization;
using System.Text;
using ClientSamgkOutputResponse.Interfaces.Schedule;

namespace GeneratorDuty.Utils;

public static class ScheduleUtils
{
    public static string GetStringFromRasp(this IResultOutScheduleFromDate scheduleFromDate)
    {
        var msg = new StringBuilder();

        msg.AppendLine($"Расписание на {scheduleFromDate.Date.ToString("dd.MM")} | {CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(scheduleFromDate.Date.DayOfWeek)}");
        
        foreach (var lesson in scheduleFromDate.Lessons)
        {
            string teachers = lesson.Identity.Aggregate(string.Empty, (current, teacher) => current + (lesson.Identity.Count >= 2 ? $"{teacher.ShortName}," : $"{teacher.ShortName}"));
            string cabs = lesson.Cabs.Aggregate(string.Empty, (current, cab) => current + (lesson.Cabs.Count >= 2 ? $"{cab.Auditory}," : $"{cab.Auditory}"));
            msg.AppendLine($"<blockquote>{lesson.NumPair}.{lesson.NumLesson} | <b>{lesson.DurationStart.ToString()}-{lesson.DurationEnd.ToString()}</b>");
            var isAttestation = lesson.SubjectDetails.IsAttestation ? "<b>[Дифф. зачёт]</b> " : string.Empty;
            msg.AppendLine($"{isAttestation}{lesson.SubjectDetails.SubjectName}");
            msg.AppendLine($"{teachers}");
            msg.AppendLine($"Каб: {cabs} • {lesson.EducationGroup.Name}</blockquote>");
        }

        if (scheduleFromDate.Lessons.Count is 0)
        {
            msg.AppendLine($"\n<blockquote> Расписание еще не внесено</blockquote>");
        }

        return msg.ToString();
    }
}