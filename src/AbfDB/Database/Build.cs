using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AbfDB.Database;

public static class Build
{
    public static void FromScratch(string searchFolder, string dbFilePath)
    {
        Stopwatch sw = Stopwatch.StartNew();
        List<AbfRecord> abfRecords = new();
        string[] abfPaths = WindowsSearch.FindIndexedAbfs(searchFolder).Select(x => x.Key).ToArray();
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
}