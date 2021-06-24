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

        public override void AddAbf(string path, int episodes, uint date, uint time, int stopwatch, string md5)
        {
            string folder = Path.GetDirectoryName(path);
            string filename = Path.GetFileName(path);
            StreamWriter.WriteLine($"\"{folder}\",\"{filename}\",{episodes},{date},{time},{stopwatch},{md5}");
        }

        public override void Dispose()
        {
            StreamWriter.Close();
            StreamWriter.Dispose();
            Console.WriteLine($"Wrote: {FilePath}");
        }
    }
}
