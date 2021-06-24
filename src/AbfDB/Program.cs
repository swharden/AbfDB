using System;
using System.Diagnostics;
using System.IO;
using AbfDB.Databases;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace AbfDB
{
    class Program
    {
        static readonly MD5 MD5 = MD5.Create();

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
                    string fileHash = GetMD5Hash(fullPath);

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
                            stopwatch: abf.Header.lStopwatchTime,
                            md5: fileHash
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

        private static string GetMD5Hash(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            return string.Join("", MD5.ComputeHash(stream).Select(x => x.ToString("x2")));
        }
    }
}
