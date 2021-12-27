using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AbfDB
{
    internal static class TsvDatabase
    {
        internal static void SqliteFromTsv(string tsvPath, string dbPath)
        {
            using AbfDatabase db = new(dbPath);

            string[] lines = File.ReadAllLines(tsvPath);

            for (int i = 1; i < lines.Length; i++)
            {
                double percent = i * 100.0 / lines.Length;
                Console.WriteLine($"Adding {i:N0} of {lines.Length:N0} ({percent:0.00}%)");

                string[] parts = lines[i].Split("\t");

                AbfRecord abfRecord = new();
                abfRecord.Folder = parts[0];
                abfRecord.Filename = parts[1];
                abfRecord.Guid = parts[2];
                abfRecord.Recorded = ParseAbfDate(parts[5]);
                abfRecord.Protocol = parts[6];
                abfRecord.LengthSec = double.Parse(parts[4]);
                abfRecord.Comments = parts[7];

                db.Add(abfRecord);
            }
        }

        internal static DateTime ParseAbfDate(string dateCode)
        {
            if (dateCode.Length != 8)
                throw new InvalidOperationException();

            int year = int.Parse(dateCode[0..4]);
            int month = int.Parse(dateCode[4..6]);
            int day = int.Parse(dateCode[6..8]);

            return new DateTime(year, month, day);
        }
    }
}
