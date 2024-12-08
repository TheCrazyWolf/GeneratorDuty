using System.Globalization;
using System.Text;
using ClientSamgk.Enums;
using ClientSamgkOutputResponse.Interfaces.Schedule;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneratorDuty.Utils;

public static class ScheduleUtils
{
    public static string GetStringFromRasp(this IResultOutScheduleFromDate scheduleFromDate)
    {
        var msg = new StringBuilder();
        msg.AppendLine(
            $"Расписание на {scheduleFromDate.Date.ToString("dd.MM.yyyy")} | {CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(scheduleFromDate.Date.DayOfWeek).ToUpperFirstLetter()}");

        msg.AppendLine(scheduleFromDate.Lessons.Count is 0
            ? "<blockquote> Расписание еще не внесено</blockquote>"
            : scheduleFromDate.Lessons.GetStringFromLesson());
        
        return msg.ToString();
    }
    
    public static string ToUpperFirstLetter(this string source)
    {
        if (string.IsNullOrEmpty(source))
            return string.Empty;
        // convert to char array of the string
        char[] letters = source.ToCharArray();
        // upper case the first char
        letters[0] = char.ToUpper(letters[0]);
        // return the array made of the new char array
        return new string(letters);
    }

    public static string GetStringFromLesson(this IList<IResultOutLesson> lessons)
    {
        var msg = new StringBuilder();
        
        foreach (var lesson in lessons)
        {
            string teachers = lesson.Identity.Aggregate(string.Empty,
                (current, teacher) =>
                    current + (lesson.Identity.Count >= 2 ? $"{teacher.ShortName}," : $"{teacher.ShortName}"));
            string cabs = lesson.Cabs.Aggregate(string.Empty,
                (current, cab) => current + (lesson.Cabs.Count >= 2 ? $"{cab.Auditory}," : $"{cab.Auditory}"));
            
            string durations = lesson.Durations.Aggregate(string.Empty,
                (current, duration) => current + (lesson.Cabs.Count >= 1 ? $"{duration.StartTime.ToString()} - {duration.EndTime.ToString()};  " : $"{duration.StartTime.ToString()} - {duration.EndTime.ToString()}"));

            string numpair = lesson.NumLesson is 0 ? $"{lesson.NumPair}" : $"{lesson.NumPair}.{lesson.NumLesson}";
            
            msg.AppendLine(
                $"<blockquote><b>{numpair}</b> | <b>{durations}</b>");
            var isAttestation = lesson.SubjectDetails.IsAttestation ? "<b>[Дифф. зачёт]</b> " : string.Empty;
            msg.AppendLine($"{isAttestation}{lesson.SubjectDetails.FullSubjectName}");
            msg.AppendLine($"{teachers}");
            msg.AppendLine($"Каб: <b>{cabs}</b> • {lesson.EducationGroup?.Name}</blockquote>");
        }

        return msg.ToString();
    }

    public static string GetMd5(this IResultOutScheduleFromDate scheduleFromDate)
    {
        // Use input string to calculate MD5 hash
        using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        
        byte[] inputBytes = Encoding.ASCII.GetBytes(scheduleFromDate.Lessons.GetStringFromLesson());
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes);
    }

    public static IList<IList<InlineKeyboardButton>> GenerateKeyboardOnSchedule(
        this IResultOutScheduleFromDate scheduleFromDate,
        ScheduleSearchType type, string value)
    {
        // example: schedule <type> <value> <date>
        return new List<IList<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("👈",
                    $"schedule {type} {value} {scheduleFromDate.Date.AddDays(-1):dd.MM.yyyy}"),
                InlineKeyboardButton.WithCallbackData("❌",
                    $"schedule clear"),
                InlineKeyboardButton.WithCallbackData("♻️",
                    $"schedule {type} {value} {scheduleFromDate.Date:dd.MM.yyyy}"),
                InlineKeyboardButton.WithCallbackData("👉",
                    $"schedule {type} {value} {scheduleFromDate.Date.AddDays(+1):dd.MM.yyyy}"),
            },
        };
    }
}