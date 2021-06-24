using System;
using System.IO;

namespace AbfDB.Databases
{
    public class CsvDatabase : AbfDatabase
    {
        private readonly StreamWriter StreamWriter;
        private readonly string FilePath;

        public CsvDatabase(string filename)
        {
            FilePath = Path.GetFullPath(filename);
            StreamWriter = File.CreateText(filename);
            StreamWriter.WriteLine("'Folder','Filename','Episodes','Date','Time','Stopwatch','MD5 Hash'".Replace("'", "\""));
        }

        public override void Add(AbfRecord record)
        {
            string folder = Path.GetDirectoryName(record.FullPath);
            string filename = Path.GetFileName(record.FullPath);
            StreamWriter.WriteLine($"\"{folder}\",\"{filename}\",{record.Episodes},{record.Date},{record.Time},{record.Stopwatch},{record.FileHashMD5}");
        }

        public override void Dispose()
        {
            StreamWriter.Close();
            StreamWriter.Dispose();
            Console.WriteLine($"Wrote: {FilePath}");
        }
    }
}
