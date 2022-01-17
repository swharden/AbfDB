using System;
using System.IO;

namespace AbfDB.Database;

/// <summary>
/// Represents a row in the database
/// </summary>
public record AbfRecord
{
    public string Folder = string.Empty;
    public string Filename = string.Empty;
    public string FullPath => Path.Combine(Folder, Filename);
    public int SizeBytes = -1;
    public string Guid = "0000-0000-0000-0000";
    public DateTime Recorded;
    public DateTime Modified;
    public int RecordedDay => int.Parse($"{Recorded.Year:0000}" + $"{Recorded.Month:00}" + $"{Recorded.Day:00}");
    public DateTime Logged;
    public int LoggedDay => int.Parse($"{Logged.Year:0000}" + $"{Logged.Month:00}" + $"{Logged.Day:00}");
    public string Protocol = string.Empty;
    public double LengthSec = -1;
    public string Comments = string.Empty;
}
