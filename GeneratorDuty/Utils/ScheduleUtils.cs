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
        
        foreach (var lesson in scheduleFromDate.Lessons
                     .GroupBy(l => new { l.NumPair, l.NumLesson, l.SubjectDetails.SubjectName})
                     .Select(g => g.First())
                     .ToList())
        {
            msg.AppendLine($"<blockquote>{lesson.NumPair}.{lesson.NumLesson}");
            msg.AppendLine($"{GetShortDiscipline(lesson.SubjectDetails.SubjectName)}");
            msg.AppendLine($"{GetPrepareStringTeacher(lesson.Identity.First().Name)}");
            msg.AppendLine($"Каб: {lesson.Cabs.FirstOrDefault()?.Adress} • {lesson.EducationGroup.Name}</blockquote>");
        }

        return msg.ToString();
    }
    
    public static string GetPrepareStringTeacher(string teacherName)
    {
        teacherName = teacherName.Replace("  ", string.Empty)
            .Replace("  ", string.Empty);
        
        var arraysTeacherName = teacherName.Split(' ');

        if (arraysTeacherName.Length == 3)
            return $"{arraysTeacherName[0]} {arraysTeacherName[1].FirstOrDefault()}. {arraysTeacherName[2].FirstOrDefault()}.";

        return teacherName;
    }

    public static string GetShortDiscipline(string disciplineName)
    {
        var arraysDisciplineName = disciplineName.Split(' ');

        if (arraysDisciplineName.Length <= 3)
            return disciplineName;

        return $"{arraysDisciplineName[0]} {arraysDisciplineName[1]} {arraysDisciplineName[2]}...";
    }
}