using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace AbfDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                ShowInvalidArgumentMessage();
                return;
            }

            string command = args[0];
            string searchFolder = args[1];
            string dbFilePath = args[2];

            if (command == "update")
                UpdateDatabaseFromIndexedFilesystem(searchFolder, dbFilePath);
            else if (command == "build")
                BuildDatabaseFromScratch(searchFolder, dbFilePath);
            else
                ShowInvalidArgumentMessage();

        }

        private static void ShowInvalidArgumentMessage()
        {
            Console.WriteLine("ERROR: invalid arguments.");
            Console.WriteLine("");
            Console.WriteLine("To update an existing database:");
            Console.WriteLine("  AbfDB.exe update [searchPath] [dbFilePath]");
            Console.WriteLine("");
            Console.WriteLine("To build a database from scratch:");
            Console.WriteLine("  AbfDB.exe build [searchPath] [dbFilePath]");
            Console.WriteLine("");
        }

        private static void BuildDatabaseFromScratch(string searchFolder, string dbFilePath)
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<AbfRecord> abfRecords = new();
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
            Console.WriteLine($"Completed in {sw.Elapsed}");
        }

        private static void UpdateDatabaseFromIndexedFilesystem(string searchFolder, string dbFilePath)
        {
            Stopwatch sw = Stopwatch.StartNew();

            Dictionary<string, AbfRecord> fsABFs = FindIndexedAbfs(searchFolder);

            Database.AbfDatabase db = new(dbFilePath);
            Dictionary<string, AbfRecord> dbABFs = db.GetIndexedAbfs().ToDictionary(x => x.FullPath);

            List<string> AbfsToAdd = new();
            List<string> AbfsToRemove = new();

            foreach (string fsAbfPath in fsABFs.Keys)
            {
                if (!dbABFs.ContainsKey(fsAbfPath))
                {
                    Console.WriteLine($"in filesystem but not database: {fsAbfPath}");
                    AbfsToAdd.Add(fsAbfPath);
                    continue;
                }

                var fsABF = fsABFs[fsAbfPath];
                var dbABF = dbABFs[fsAbfPath];
                var timeDifference = fsABF.Modified - dbABF.Modified;
                if (timeDifference.Hours > 24)
                {
                    Console.WriteLine();
                    Console.WriteLine($"mismatch timestamp: {fsAbfPath}");
                    AbfsToRemove.Add(fsAbfPath);
                    AbfsToAdd.Add(fsAbfPath);
                    continue;
                }
            }

            foreach (string dbAbfPath in dbABFs.Keys)
            {
                if (!fsABFs.ContainsKey(dbAbfPath))
                {
                    Console.WriteLine($"in database but not filesystem: {dbAbfPath}");
                    AbfsToRemove.Add(dbAbfPath);
                    continue;
                }
            }

            db.Remove(AbfsToRemove.ToArray());
            db.Add(AbfsToAdd.ToArray());
            int modifiedRecordCount = AbfsToRemove.Count + AbfsToAdd.Count;
            Console.WriteLine($"Modified {modifiedRecordCount:N0} records in the database");

            Console.WriteLine($"Completed in {sw.Elapsed}");
        }

        private static Dictionary<string, AbfRecord> FindIndexedAbfs(string searchFolder)
        {
            Console.WriteLine("Searching filesystem for ABF files...");
            Dictionary<string, AbfRecord> abfs = WindowsSearch.FindAbfs(searchFolder);
            Console.WriteLine($"Located {abfs.Count:N0} ABFs in the filesystem.");
            return abfs;
        }
    }
}