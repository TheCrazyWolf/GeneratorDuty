using System.Text;
using ClientSamgk.Enums;
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
            $"<tr> <th>Пара</th> <th>Время</th> <th>Дисциплина/преподаватель</th> <th>Кабинет</th> </tr>";
        _rows += row;
    }
    
    public void AddRow(IResultOutScheduleFromDate result, ScheduleSearchType searchType)
    {
        _rows += $"<td style=\"text-align: center;\" colspan=\"4\">{GetRowTitle(result, searchType)}</td>";
        
        foreach (var item in result.Lessons)
        {
            var teachers = string.Empty;
            var cabs = string.Empty;

            teachers = item.Identity.Aggregate(teachers, (current, teacher) => current + $"<br>{teacher.Name}");
            cabs = item.Cabs.Aggregate(cabs, (current, cab) => current + $"<br>{cab.Adress}");

            var row = $"<tr> <td>{item.NumPair}.{item.NumLesson}</td> <td>{item.DurationStart} -<br>{item.DurationEnd}</td> <td><b>{item.SubjectDetails.SubjectName}</b> {teachers}</td> <td>{cabs}</td> </tr>";
            _rows += row;
        }
    }

    public string GetRowTitle(IResultOutScheduleFromDate result, ScheduleSearchType searchType)
    {
        
        if(result.Lessons.Count is 0) return "unknow";
        
        var lessonFirst = result.Lessons.First();
        
        if (searchType == ScheduleSearchType.Employee)
            return lessonFirst.Identity.FirstOrDefault()?.Name ?? string.Empty;
        
        if (searchType == ScheduleSearchType.Group)
            return lessonFirst.EducationGroup.Name;
        
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