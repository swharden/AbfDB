using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB
{
    /// <summary>
    /// Represents a row in the database
    /// </summary>
    public record AbfRecord
    {
        public string Folder = string.Empty;
        public string Filename = string.Empty;
        public string FullPath => Path.Combine(Folder, Filename);
        public int SizeBytes = -1;
        public string Guid = string.Empty;
        public DateTime Recorded;
        public int RecordedDay => int.Parse($"{Recorded.Year:0000}" + $"{Recorded.Month:00}" + $"{Recorded.Day:00}");
        public DateTime Noted;
        public string Protocol = string.Empty;
        public double LengthSec = -1;
        public string Comments = string.Empty;
    }
}
