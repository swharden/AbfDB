using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB
{
    public static class WinSearchTSV
    {
        public static void Save(Dictionary<string, IndexedAbf> abfs, string tsvPath)
        {
            File.WriteAllText(tsvPath, string.Join(Environment.NewLine, abfs.Values.Select(x => ToTsv(x))));
        }

        public static Dictionary<string, IndexedAbf> Load(string tsvPath)
        {
            return File.ReadAllLines(tsvPath).Select(x => FromTsv(x)).ToDictionary(x => x.Path, y => y);
        }

        private static string ToTsv(IndexedAbf abf)
        {
            return $"{abf.Path}\t{abf.SizeBytes}\t{abf.HasRSV}\t{abf.Modified}";
        }

        private static IndexedAbf FromTsv(string line)
        {
            string[] parts = line.Trim().Split("\t");
            if (parts.Length != 4)
                throw new ArgumentException($"does not contain 4 TSV values: {line}");

            return new IndexedAbf()
            {
                Path = parts[0],
                SizeBytes = int.Parse(parts[1]),
                HasRSV = bool.Parse(parts[2]),
                Modified = DateTime.Parse(parts[3]),
            };
        }
    }
}
