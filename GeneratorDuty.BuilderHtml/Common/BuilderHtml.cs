namespace GeneratorDuty.BuilderHtml.Common;

public class BuilderHtml
{
    private const string CssTable =
        "table {font-family: \"Lucida Sans Unicode\", \"Lucida Grande\", Sans-Serif;font-size: 14px;border-collapse: collapse;text-align: center;}th, td:first-child {background: #0b477b;color: white;padding: 10px 20px;}th, td {border-style: solid;border-width: 0 1px 1px 0;border-color: white;}td {background: #D8E6F3;}th:first-child, td:first-child {text-align: left;}";

    private const string Index =
        $"<!DOCTYPE html><html>  <head>    <meta charset=\"utf-8\">    <title>Отчёт</title>  <style> СSS_BEGIN </style></head>  <body>    CONTENT BEGIN   </body</html>";

    protected virtual string Content { get; set; } = string.Empty;

    protected string Render()
    {
        return Index.Replace("СSS_BEGIN", CssTable).Replace("CONTENT BEGIN", Content);
    }
}