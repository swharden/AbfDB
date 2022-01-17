using CommandLine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace AbfDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string searchFolder = @"C:\Users\swharden";
            string dbFilePath = @"C:\Users\swharden\Documents\important\abfdb\abfs.db";
            UpdateDatabaseFromIndexedFilesystem(searchFolder, dbFilePath);
        }

        private static void UpdateDatabaseFromIndexedFilesystem(string searchFolder, string dbFilePath)
        {
            Console.WriteLine("Connecting to database...");
            Database.AbfDatabase db = new(dbFilePath);
            Console.WriteLine($"Database contains {db.GetRecordCount()} records.");

            Console.WriteLine("Searching filesystem for ABF files...");
            Dictionary<string, IndexedSearch.IndexedAbf> abfs = IndexedSearch.Queries.FindAbfs(searchFolder);
            Console.WriteLine($"Located {abfs.Count} ABF files.");

            foreach (IndexedSearch.IndexedAbf abf in abfs.Values)
            {

            }
        }
    }
}