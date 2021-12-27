using System;
using System.Diagnostics;

namespace AbfDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("This program will search a folder and build a new database from scratch.");

            if (args.Length == 2)
            {
                string searchFolder = args[0];
                string basePath = System.IO.Path.Combine(args[1], DateTime.Now.Ticks.ToString());
                string tsvFile = basePath + ".tsv";
                string dbFile = basePath + ".db";
                Stopwatch watch = Stopwatch.StartNew();
                DatabaseBuilder.CreateTSV(searchFolder, tsvFile);
                DatabaseBuilder.CreateSQL(tsvFile, dbFile);
                Console.WriteLine($"Total time to scan and build the database: {watch.Elapsed}");
            }
            else
            {
                Console.WriteLine("ERROR: Invalid arguments. Use like this:");
                Console.WriteLine("  AbfDB.exe X:/database/input/ C:/database/output/");
            }
        }
    }
}