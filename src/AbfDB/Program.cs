using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Data.Sqlite;

namespace AbfDB
{
    class Program
    {
        static int Abfs;
        static int Folders;
        static int Errors;
        static Stopwatch Stopwatch;

        static void Main(string[] args)
        {
            string rootFolder;
            string databaseFile;

            if (Debugger.IsAttached)
            {
                rootFolder = @"X:\Data\SD\OXT-Subiculum\Dose Experiments\10 nM 10 min exposure";
                databaseFile = "test.db";
            }
            else if (args.Length == 2)
            {
                rootFolder = Path.GetFullPath(args[0]);
                databaseFile = Path.GetFullPath(args[1]);
            }
            else
            {
                Console.WriteLine("ERROR: 2 arguments required (scan path and output file).");
                Console.WriteLine("Example: abfdb.exe \"c:\\scan\\folder\" \"c:\\output.db\"");
                return;
            }

            Stopwatch = Stopwatch.StartNew();

            SqliteConnection conn = new($"Data Source={databaseFile};");
            conn.Open();

            Tables.Abfs.Create(conn);
            Tables.Errors.Create(conn);
            Tables.Scans.Create(conn);
            RecursivelyAddAbfs(conn, rootFolder);
            Tables.Scans.Add(conn, rootFolder, Stopwatch, Abfs, Folders, Errors);

            conn.Close();
            Console.WriteLine($"Wrote: {databaseFile}");
        }

        static void RecursivelyAddAbfs(SqliteConnection conn, string folderPath)
        {
            Folders += 1;
            DirectoryInfo di = new(folderPath);

            foreach (FileInfo abfFile in di.GetFiles("*.abf"))
            {
                string abfFilePath = abfFile.FullName;

                Console.WriteLine($"[{Stopwatch.Elapsed}] [ABFs={Abfs}] {abfFilePath}");

                try
                {
                    Tables.Abfs.Add(conn, abfFilePath);
                    Abfs += 1;
                }
                catch (Exception ex)
                {
                    Errors += 1;
                    Console.WriteLine($"EXCEPTION: {ex.Message}");
                    Tables.Errors.Add(conn, abfFilePath, ex);
                }
            }

            foreach (DirectoryInfo subFolder in di.GetDirectories())
                RecursivelyAddAbfs(conn, subFolder.FullName);
        }
    }
}
