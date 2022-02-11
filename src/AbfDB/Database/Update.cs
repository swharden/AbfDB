using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AbfDB.Database;

public static class Update
{
    public static void FromIndexedAbfs(Dictionary<string, AbfRecord> fsABFs, string dbFilePath)
    {
        Stopwatch sw = Stopwatch.StartNew();

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
}
