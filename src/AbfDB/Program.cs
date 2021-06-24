using System;
using System.Diagnostics;
using System.IO;
using AbfDB.Databases;

namespace AbfDB
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Debugger.IsAttached)
                args = new string[] { @"..\..\..\..\..\..\pyABF\data\abfs" };

            if (args.Length != 1 || !Directory.Exists(args[0]))
            {
                Console.WriteLine("ERROR: command line argument must be a valid search folder");
                return;
            }

            using CsvDatabase csv = new("abfdb.csv");
            using SqliteDatabase sql = new("abfdb.sqlite");
            AbfDatabase[] databases = { csv, sql };

            int count = 0;
            Stopwatch sw = Stopwatch.StartNew();

            if (File.Exists("log.txt"))
                File.Delete("log.txt");
            using StreamWriter log = File.AppendText("log.txt");

            foreach (string path in Directory.EnumerateFiles(args[0], "*.abf", SearchOption.AllDirectories))
            {
                if (path.EndsWith(".abf") == false)
                    continue;

                string fullPath = Path.GetFullPath(path);
                Console.WriteLine($"ABFs={count++} Elapsed={sw.Elapsed} Path={fullPath}");

                try
                {
                    // Use ABFFIO.DLL - benchmark scan of 628 ABFs (over network) took 18.04 seconds
                    AbfSharp.ABFFIO.ABF abf = new(fullPath);

                    // Use native C# - benchmark scan of 628 ABFs (over network) took 11.16 seconds
                    //AbfSharp.ABF abf = new(fullPath);

                    foreach (AbfDatabase database in databases)
                    {
                        database.AddAbf(
                            path: fullPath,
                            episodes: abf.Header.lActualEpisodes,
                            date: abf.Header.uFileStartDate,
                            time: abf.Header.uFileStartTimeMS,
                            stopwatch: abf.Header.lStopwatchTime
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EXCEPTION: see log.txt for details");
                    log.WriteLine($"{fullPath}\n{ex}\n");
                    log.Flush();
                }
            }

            log.WriteLine($"Stored {count} ABFs in {sw.Elapsed}");
        }
    }
}
