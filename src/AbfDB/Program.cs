using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Data.Sqlite;

namespace AbfDB
{
    class Program
    {
        static int Abfs = 0;
        static int Folders = 0;
        static int Errors = 0;

        static void Main(string[] args)
        {
            string timestamp = DateTime.Now.ToOADate().ToString().Replace(".", "");
            string dbFilePath = Path.GetFullPath($"abfdb-{timestamp}.db");
            Stopwatch sw = Stopwatch.StartNew();

            SqliteConnection conn = new($"Data Source={dbFilePath};");
            conn.Open();

            Tables.Abfs.Create(conn);
            Tables.Errors.Create(conn);
            Tables.Scans.Create(conn);

            if (Debugger.IsAttached)
            {
                ScanFolder(conn, @" X:\Data\2P01\2010\08-2010\08-12-2010-CL"); // has bad ABF
                ScanFolder(conn, @"X:\Data\AT1-Cre-AT2-eGFP\aorta-ChR2-tdTomato");
                ScanFolder(conn, @"X:\Data\DIC1\2006\02-2006\02-02-2006-BN");
                Tables.Scans.Add(conn, "custom", sw, Abfs, Folders, Errors);
            }
            else
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("ERROR: command line argument (scan folder) required.");
                    return;
                }

                string scanFolder = args[0];
                ScanFolder(conn, scanFolder);
                Tables.Scans.Add(conn, scanFolder, sw, Abfs, Folders, Errors);
            }

            conn.Close();
            Console.WriteLine($"Wrote: {dbFilePath}");
        }

        static void ScanFolder(SqliteConnection conn, string folderPath)
        {
            Folders += 1;
            DirectoryInfo di = new(folderPath);

            foreach (FileInfo abfFile in di.GetFiles("*.abf"))
            {
                string abfFilePath = abfFile.FullName;

                Console.WriteLine(abfFilePath);
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
                ScanFolder(conn, subFolder.FullName);
        }
    }
}
