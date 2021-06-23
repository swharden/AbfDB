using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

            string[] abfFilePaths = Directory.GetFiles(args[0])
                .Where(x => x.EndsWith(".abf"))
                .Select(x => Path.GetFullPath(x))
                .ToArray();

            int count = 0;
            Stopwatch sw = Stopwatch.StartNew();

            foreach (string path in abfFilePaths)
            {
                Console.WriteLine($"ABFs={count++} Elapsed={sw.Elapsed} Path={path}");
                AbfSharp.ABFFIO.ABF abf = new(path);

                foreach (AbfDatabase database in databases)
                {
                    database.AddAbf(
                        path: path,
                        episodes: abf.Header.lActualEpisodes,
                        date: abf.Header.uFileStartDate,
                        time: abf.Header.uFileStartTimeMS,
                        stopwatch: abf.Header.lStopwatchTime
                    );
                }
            }
        }
    }
}
