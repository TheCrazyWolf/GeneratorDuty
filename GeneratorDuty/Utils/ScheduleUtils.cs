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
            $"Расписание на {scheduleFromDate.Date.ToString("dd.MM")} | {CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(scheduleFromDate.Date.DayOfWeek)}");

        foreach (var lesson in scheduleFromDate.Lessons)
        {
            string teachers = lesson.Identity.Aggregate(string.Empty,
                (current, teacher) =>
                    current + (lesson.Identity.Count >= 2 ? $"{teacher.ShortName}," : $"{teacher.ShortName}"));
            string cabs = lesson.Cabs.Aggregate(string.Empty,
                (current, cab) => current + (lesson.Cabs.Count >= 2 ? $"{cab.Auditory}," : $"{cab.Auditory}"));
            msg.AppendLine(
                $"<blockquote>{lesson.NumPair}.{lesson.NumLesson} | <b>{lesson.DurationStart.ToString()}-{lesson.DurationEnd.ToString()}</b>");
            var isAttestation = lesson.SubjectDetails.IsAttestation ? "<b>[Дифф. зачёт]</b> " : string.Empty;
            msg.AppendLine($"{isAttestation}{lesson.SubjectDetails.SubjectName}");
            msg.AppendLine($"{teachers}");
            msg.AppendLine($"Каб: {cabs} • {lesson.EducationGroup.Name}</blockquote>");
        }

        if (scheduleFromDate.Lessons.Count is 0)
        {
            msg.AppendLine($"<blockquote> Расписание еще не внесено</blockquote>");
        }

        return msg.ToString();
    }

    public static IList<IList<InlineKeyboardButton>> GenerateKeyboardOnSchedule(this IResultOutScheduleFromDate scheduleFromDate,
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
                    $"schedule_close"),
                InlineKeyboardButton.WithCallbackData("👉",
                    $"schedule {type} {value} {scheduleFromDate.Date.AddDays(+1):dd.MM.yyyy}"),
            },
        };
    }
}