using System.Text;
using ClientSamgk;
using ClientSamgkOutputResponse.Interfaces.Schedule;
using GeneratorDuty.Common;
using GeneratorDuty.Database;
using Telegram.Bot;

namespace GeneratorDuty.BackgroundServices;

public class AutoSendSchedule(ITelegramBotClient client, DutyContext ef) : BaseTask
{
    private ClientSamgkApi _clientSamgkApi = new ClientSamgkApi();
    public override Task RunAsync()
    {
        Task.Run(WorkSerivce);
        return Task.CompletedTask;
    }

    private async Task WorkSerivce()
    {
        while (true)
        {
            if(!CanWorkSerivce(DateTime.Now)) continue;

            var g = ef.ScheduleProps
                .Where(x => x.IsAutoSend).ToList();
            
            foreach (var item in g)
            {
                var dateTime = DateTime.Now;

                // если время вечернее смотрим расписание на перед
                if (dateTime.Hour >= 18)
                    dateTime = dateTime.AddDays(1);
                
                // если день выходной, то пропускаем и добавляем дни пока не попадется рабочий
                while (dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                {
                    dateTime = dateTime.AddDays(1);
                }

                var result = await _clientSamgkApi.Schedule
                    .GetScheduleAsync(DateOnly.FromDateTime(dateTime), item.SearchType, item.Value);
                
                item.LastResult = GetStringFromRasp(result);
                await client.SendTextMessageAsync(item.IdPeer, item.LastResult);
                
                await Task.Delay(1000);
            }
            
            await Task.Delay(1000);
        }
    }
    
    
    private bool CanWorkSerivce(DateTime nowTime)
    {
        return true;
    }
    
    protected string GetStringFromRasp(IResultOutScheduleFromDate scheduleFromDate)
    {
        var msg = new StringBuilder();

        msg.AppendLine($"Расписание на {scheduleFromDate.Date}");
        
        foreach (var lesson in scheduleFromDate.Lessons
                     .GroupBy(l => new { l.NumPair, l.NumLesson, l.SubjectDetails.SubjectName})
                     .Select(g => g.First())
                     .ToList())
        {
            msg.AppendLine($"=====================");
            msg.AppendLine($"{lesson.NumPair}.{lesson.NumLesson}");
            msg.AppendLine($"{GetShortDiscipline(lesson.SubjectDetails.SubjectName)}");
            msg.AppendLine($"{GetPrepareStringTeacher(lesson.Identity.First().Name)}");
            msg.AppendLine($"Каб: {lesson.Cabs.FirstOrDefault()?.Adress} • {lesson.EducationGroup.Name}");
        }

        return msg.ToString();
    }
    
    protected string GetPrepareStringTeacher(string teacherName)
    {
        teacherName = teacherName.Replace("  ", string.Empty)
            .Replace("  ", string.Empty);
        
        var arraysTeacherName = teacherName.Split(' ');

        if (arraysTeacherName.Length == 3)
            return $"{arraysTeacherName[0]} {arraysTeacherName[1].FirstOrDefault()}. {arraysTeacherName[2].FirstOrDefault()}.";

        return teacherName;
    }

    protected string GetShortDiscipline(string disciplineName)
    {
        var arraysDisciplineName = disciplineName.Split(' ');

        if (arraysDisciplineName.Length <= 3)
            return disciplineName;

        return $"{arraysDisciplineName[0]} {arraysDisciplineName[1]} {arraysDisciplineName[2]}...";
    }
}