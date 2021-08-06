using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AbfDB
{
    public class DatabaseBuilder
    {
        public readonly List<AbfInfo> AbfInfos = new();

        public DatabaseBuilder()
        {

        }

        public void LoadTsv(string tsvPath)
        {
            if (!File.Exists(tsvPath))
                throw new FileNotFoundException(tsvPath);

            string[] lines = File.ReadAllLines(tsvPath);
            for (int i = 1; i < lines.Length; i++)
            {
                AbfInfo abf = TsvFile.ParseLine(lines[i]);
                AbfInfos.Add(abf);
            }

            Console.WriteLine($"Loaded information for {AbfInfos.Count} ABFs");
        }

        public void SaveDatabase(string dbPath)
        {
            throw new NotImplementedException();
        }
    }
}
