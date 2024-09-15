using System.Globalization;
using System.Text;
using ClientSamgkOutputResponse.Implementation.Education;
using ClientSamgkOutputResponse.Implementation.Schedule;
using ClientSamgkOutputResponse.Interfaces.Schedule;

namespace GeneratorDuty.Utils;

public static class ScheduleUtils
{
    public static string GetStringFromRasp(this IResultOutScheduleFromDate scheduleFromDate)
    {
        var msg = new StringBuilder();

        msg.AppendLine($"Расписание на {scheduleFromDate.Date} ({CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(scheduleFromDate.Date.DayOfWeek)})");

        var lessons = scheduleFromDate.Lessons
            .GroupBy(l => new
            {
                l.NumPair,
                l.NumLesson,
                l.SubjectDetails.SubjectName
            })
            .Select(g => g.First())
            .ToList();

        if (lessons.Count is not 0 && scheduleFromDate.Date.DayOfWeek is DayOfWeek.Monday)
        {
            var firstLesson = lessons.First();

            if (firstLesson.NumPair is 1 && firstLesson.NumLesson is 1 || firstLesson.NumPair is 1 && firstLesson.NumLesson is 0)
            {
                lessons.Add(new ResultOutResultOutLesson
                {
                    NumLesson = 0, NumPair = 0,
                    SubjectDetails = new ResultOutSubject
                    {
                        Id = 0,
                        SubjectName = "Классный час. Разговоры о важном"
                    },
                    Cabs = firstLesson.Cabs,
                    EducationGroup = firstLesson.EducationGroup,
                    Identity = firstLesson.Identity
                });
            }

            lessons = lessons.OrderBy(x => x.NumPair)
                .ThenBy(x => x.NumPair).ToList();
        }
        
        foreach (var lesson in lessons)
        {
            msg.AppendLine($"<blockquote><b>{lesson.GetDurationLesson(scheduleFromDate.Date.DayOfWeek)}</b> ({lesson.NumPair}.{lesson.NumLesson})");
            msg.AppendLine($"{lesson.SubjectDetails.SubjectName}");
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

    public static string GetDurationLesson(this IResultOutLesson lesson, DayOfWeek dayOfWeek)
    {
        switch (dayOfWeek)
        {
            case DayOfWeek.Monday when lesson.NumPair is 0 && lesson.NumLesson is 0:
                return "08.25 - 09.10";
            case DayOfWeek.Monday when lesson.NumPair is 1 && lesson.NumLesson is 0:
                return "09.15 - 10.55";
            case DayOfWeek.Monday when lesson.NumPair is 1 && lesson.NumLesson is 1:
                return "09.15 - 10.00";
            case DayOfWeek.Monday when lesson.NumPair is 1 && lesson.NumLesson is 2:
                return "10.10 - 10.55";
            case DayOfWeek.Monday when lesson.NumPair is 2 && lesson.NumLesson is 0:
                return "11.00 - 13.00";
            case DayOfWeek.Monday when lesson.NumPair is 2 && lesson.NumLesson is 1:
                return "11.00 - 11.45";
            case DayOfWeek.Monday when lesson.NumPair is 2 && lesson.NumLesson is 2:
                return "12.15 - 13.00";
            case DayOfWeek.Monday when lesson.NumPair is 3 && lesson.NumLesson is 0:
                return "13.05 - 14.45";
            case DayOfWeek.Monday when lesson.NumPair is 3 && lesson.NumLesson is 1:
                return "13.05 - 13.50";
            case DayOfWeek.Monday when lesson.NumPair is 3 && lesson.NumLesson is 2:
                return "14.00- 14.45";
            case DayOfWeek.Monday when lesson.NumPair is 4 && lesson.NumLesson is 0:
                return "14.50 - 16.30";
            case DayOfWeek.Monday when lesson.NumPair is 4 && lesson.NumLesson is 1:
                return "14.50 - 15.35";
            case DayOfWeek.Monday when lesson.NumPair is 4 && lesson.NumLesson is 2:
                return "15.45 - 16.30";
            case DayOfWeek.Monday when lesson.NumPair is 5 && lesson.NumLesson is 0:
                return "16.35 - 18.15";
            case DayOfWeek.Monday when lesson.NumPair is 5 && lesson.NumLesson is 1:
                return "16.35 - 17.20";
            case DayOfWeek.Monday when lesson.NumPair is 5 && lesson.NumLesson is 2:
                return "17.30 - 18.15";
            case DayOfWeek.Monday when lesson.NumPair is 6 && lesson.NumLesson is 0:
                return "18.20 - 20.00";
            case DayOfWeek.Monday when lesson.NumPair is 6 && lesson.NumLesson is 1:
                return "18.20 - 19.05";
            case DayOfWeek.Monday when lesson.NumPair is 6 && lesson.NumLesson is 2:
                return "19.15 - 25.00";
        }

        switch (lesson.NumPair)
        {
            case 1 when lesson.NumLesson is 0:
                return "08.25 - 10.00";
            case 1 when lesson.NumLesson is 1:
                return "08.25 - 09.10";
            case 1 when lesson.NumLesson is 2:
                return "09.15 - 10.00";
            case 2 when lesson.NumLesson is 0:
                return "10.10 - 11.45";
            case 2 when lesson.NumLesson is 1:
                return "10.10 - 10.55";
            case 2 when lesson.NumLesson is 2:
                return "11.00 - 11.45";
            case 3 when lesson.NumLesson is 0:
                return "12.15 - 13.50";
            case 3 when lesson.NumLesson is 1:
                return "12.15 - 13.00";
            case 3 when lesson.NumLesson is 2:
                return "13.05 - 13.50";
            case 4 when lesson.NumLesson is 0:
                return "14.00 - 15.35";
            case 4 when lesson.NumLesson is 1:
                return "14.00 - 14.45";
            case 4 when lesson.NumLesson is 2:
                return "14.50 - 15.35";
            case 5 when lesson.NumLesson is 0:
                return "15.45 - 17.20";
            case 5 when lesson.NumLesson is 1:
                return "15.45 - 16.30";
            case 5 when lesson.NumLesson is 2:
                return "16.35 - 17.20";
            case 6 when lesson.NumLesson is 0:
                return "17.30 - 19.05";
            case 6 when lesson.NumLesson is 1:
                return "17.30 - 18.15";
            case 6 when lesson.NumLesson is 2:
                return "18.20 - 19.05";
            case 7 when lesson.NumLesson is 0:
                return "19.15 - 20.50";
            case 7 when lesson.NumLesson is 1:
                return "19.15 - 20.00";
            case 7 when lesson.NumLesson is 2:
                return "20.05 - 20.05";
            default:
                return "undefined";
        }
    }
}