using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace AbfDB
{
    public class Program
    {
        static readonly Stopwatch Watch = Stopwatch.StartNew();

        public static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                string searchFolder = args[0];
                string dbFilePath = args[1];
                BuildDatabaseFromScratch(searchFolder, dbFilePath);
            }
            else if (args.Length == 0)
            {
                Console.WriteLine("WARNING: RUNNING DEVELOPER TEST");
                string searchFolder = @"C:\Users\swharden";
                string dbFilePath = @"C:\Users\swharden\Documents\important\abfdb\abfs.db";
                UpdateDatabaseFromIndexedFilesystem(searchFolder, dbFilePath);
            }
            else
            {
                Console.WriteLine("ERROR: invalid arguments");
            }

            Console.WriteLine($"Completed in {Watch.Elapsed}");
        }

        private static void BuildDatabaseFromScratch(string searchFolder, string dbFilePath)
        {
            List<Database.AbfRecord> abfRecords = new();
            string[] abfPaths = FindIndexedAbfs(searchFolder).Select(x => x.Key).ToArray();
            for (int i = 0; i < abfPaths.Length; i++)
            {
                string abfPath = abfPaths[i];
                Console.WriteLine($"Analyzing {i + 1:N0} of {abfPaths.Length:N0}: {abfPath}");
                abfRecords.Add(AbfFile.GetRecord(abfPath));
            }

            Console.WriteLine("Connecting to database...");
            Database.AbfDatabase db = new(dbFilePath);
            Console.WriteLine($"Database contains {db.GetRecordCount()} records.");
            Console.WriteLine($"Adding {abfRecords.Count} records...");
            db.Add(abfRecords.ToArray());
            Console.WriteLine($"Database contains {db.GetRecordCount()} records.");
        }

        private static void UpdateDatabaseFromIndexedFilesystem(string searchFolder, string dbFilePath)
        {
            throw new NotImplementedException();
        }

        private static Dictionary<string, IndexedSearch.IndexedAbf> FindIndexedAbfs(string searchFolder)
        {
            Console.WriteLine("Searching filesystem for ABF files...");
            Dictionary<string, IndexedSearch.IndexedAbf> abfs = IndexedSearch.Queries.FindAbfs(searchFolder);
            Console.WriteLine($"Located {abfs.Count:N0} ABF files.");
            return abfs;
        }
    }
}