using System;
using System.IO;

namespace AbfDB;

public record AbfRecord
{
    public string FullPath
    {
        get => Path.Combine(Folder, Filename);
        set
        {
            Folder = Path.GetDirectoryName(value) ?? string.Empty;
            Filename = Path.GetFileName(value);
        }
    }

    public string Folder = string.Empty;
    public string Filename = string.Empty;
    public int SizeBytes = -1;
    public string Guid = "0000-0000-0000-0000";
    public DateTime Recorded = DateTime.MinValue;
    public DateTime Modified = DateTime.MinValue;
    public int RecordedDay => int.Parse($"{Recorded.Year:0000}" + $"{Recorded.Month:00}" + $"{Recorded.Day:00}");
    public DateTime Logged;
    public int LoggedDay => int.Parse($"{Logged.Year:0000}" + $"{Logged.Month:00}" + $"{Logged.Day:00}");
    public string Protocol = string.Empty;
    public double LengthSec = -1;
    public string Comments = string.Empty;
}
