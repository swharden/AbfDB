using System;
using System.IO;

namespace AbfDB
{
    public static class TsvFile
    {
        public static AbfInfo ParseLine(string tsvLine)
        {
            string[] parts = tsvLine.Split("\t");

            int tsvColumnCount = 8;
            if (parts.Length != tsvColumnCount)
                throw new InvalidOperationException($"TSV line had {parts.Length} columns ({tsvColumnCount} expected)");

            return new AbfInfo()
            {
                FilePath = Path.Combine(parts[0], parts[1]),
                CjfGuid = parts[2],
                SampleRate = double.Parse(parts[3]),
                LengthSec = double.Parse(parts[4]),
                Date = uint.Parse(parts[5]),
                Protocol = parts[6],
                Tags = parts[7],
            };
        }

        public static string[] ColumnNames =>
            new string[] 
            {
                "Folder",
                "Filename",
                "CjfGuid",
                "SampleRate (Hz)",
                "Length (sec)",
                "Date (int code)",
                "Protocol",
                "Tags",
            };

        public static string MakeLine(AbfInfo info)
        {
            string[] parts =
            {
                Path.GetDirectoryName(info.FilePath).ToString(),
                Path.GetFileName(info.FilePath).ToString(),
                info.CjfGuid.ToString(),
                info.SampleRate.ToString(),
                info.LengthSec.ToString(),
                info.Date.ToString(),
                info.Protocol,
                info.Tags,
            };

            return string.Join("\t", parts);
        }
    }
}
