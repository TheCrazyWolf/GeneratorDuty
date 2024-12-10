using System.Text;
using ClientSamgkOutputResponse.Enums;
using ClientSamgkOutputResponse.Interfaces.Schedule;

namespace GeneratorDuty.BuilderHtml;

public class HtmlBuilderSchedule : Common.BuilderHtml
{
    protected override string Content { get; set; } = "<table> ROWS_BEGIN </table>";
    private string _rows = string.Empty;

    public HtmlBuilderSchedule()
    {
        AddRow();
    }
    
    private void AddRow()
    {
        var row =
            $"<tr> <th>Пара</th> <th>Время</th> <th>Группа</th> <th>Дисциплина/преподаватель</th> <th>Кабинет</th> </tr>";
        _rows += row;
    }
    
    public void AddRow(IResultOutScheduleFromDate result, ScheduleSearchType searchType)
    {
        _rows += $"<td style=\"text-align: center;\" colspan=\"5\">{GetRowTitle(result, searchType)}</td>";
        
        foreach (var item in result.Lessons)
        {
            var teachers = string.Empty;
            var cabs = string.Empty;
            teachers = item.Identity.Aggregate(teachers, (current, teacher) => current + $"<br>{teacher.Name}");
            cabs = item.Cabs.Aggregate(cabs, (current, cab) => current + $"<br>{cab.Adress}");
            string durations = item.Durations.Aggregate(string.Empty,
                (current, duration) => current + (item.Cabs.Count >= 1 ? $"{duration.StartTime.ToString()} - {duration.EndTime.ToString()}," : $"{duration.StartTime.ToString()} - {duration.EndTime.ToString()}"));
            
            var isAttestation = item.SubjectDetails.IsAttestation ? "<b>[Дифф. зачёт]</b> " : string.Empty;
            var row = $"<tr> <td>{item.NumPair}.{item.NumLesson}</td> <td>{durations}</td> <td>{item.EducationGroup?.Name}</td> <td>{isAttestation}<b>{item.SubjectDetails.FullSubjectName}</b> {teachers}</td> <td>{cabs}</td> </tr>";
            _rows += row;
        }
    }

    public string? GetRowTitle(IResultOutScheduleFromDate result, ScheduleSearchType searchType)
    {
        
        if(result.Lessons.Count is 0) return "unknow";
        
        var lessonFirst = result.Lessons.First();
        
        if (searchType == ScheduleSearchType.Employee)
            return lessonFirst.Identity.FirstOrDefault()?.Name ?? string.Empty;
        
        if (searchType == ScheduleSearchType.Group)
            return lessonFirst.EducationGroup?.Name;
        
        if (searchType == ScheduleSearchType.Cab)
            return lessonFirst.Cabs.FirstOrDefault()?.Adress ?? string.Empty;

        return "unknow";
    }

    public Stream GetStreamFile()
    {
        Content = Content.Replace("ROWS_BEGIN", _rows);
        return new MemoryStream(Encoding.UTF8.GetBytes(Render()));
    }
}