using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB
{
    public static class DatabaseBuilder
    {
        private static int AbfsFound;

        public static string BuildFromScratch(string scanFolder, string outFolder)
        {
            string timestamp = DateTime.Now.Ticks.ToString();
            string basePath = Path.Combine(outFolder, timestamp);
            Console.WriteLine($"Building database: {basePath}");
            string txtFile = basePath + ".txt";
            string tsvFile = basePath + ".tsv";
            string dbFile = basePath + ".db";

            Stopwatch watch = Stopwatch.StartNew();
            int abfCount = CreateFileList(scanFolder, txtFile);
            string[] abfPaths = ReadFileList(txtFile);
            CreateTSV(abfPaths, tsvFile);
            CreateSQL(tsvFile, dbFile);
            Console.WriteLine($"DONE! Analyzed {abfCount} ABFs in {watch.Elapsed}");

            return dbFile;
        }

        /// <summary>
        /// Scan a folder tree, locate all ABF files, and store them in a text file (one per line)
        /// </summary>
        public static int CreateFileList(string searchPath, string txtPath)
        {
            if (File.Exists(txtPath))
                throw new InvalidOperationException($"ERROR - file already exists: {txtPath}");

            Console.WriteLine("Scanning for ABF files...");
            AbfsFound = 0;
            using StreamWriter sw = new(txtPath);
            FindABFs(new DirectoryInfo(searchPath), sw);
            return AbfsFound;
        }

        /// <summary>
        /// Recursively scan a folder and log ABF file paths in the given stream
        /// </summary>
        private static void FindABFs(DirectoryInfo root, StreamWriter sw)
        {
            string[] filePaths = Directory
                .GetFiles(root.FullName, "*.abf")
                .Where(x => x.EndsWith(".abf", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (string filePath in filePaths)
            {
                sw.WriteLine(filePath);
                AbfsFound += 1;
                Console.WriteLine($"FOUND [{AbfsFound}] {filePath}");
            }

            foreach (DirectoryInfo dir in root.GetDirectories())
            {
                FindABFs(dir, sw);
            }
        }

        /// <summary>
        /// Return valid ABF file paths from the given text file
        /// </summary>
        private static string[] ReadFileList(string txtPath)
        {
            return File.ReadAllLines(txtPath)
                .Select(x => x.Trim())
                .Where(x => x.Length > 10)
                .ToArray();
        }

        /// <summary>
        /// Scan a folder tree and generate a new database from scratch.
        /// Recording data in a TSV file greatly improves speed over logging in a SQL database.
        /// The file buffer grows in memory and is only occasionally flushed/written.
        /// In contrast the SQL database chuggs the hard disk every time a record is inserted.
        /// </summary>
        public static void CreateTSV(string[] abfPaths, string tsvPath)
        {
            if (File.Exists(tsvPath))
                throw new InvalidOperationException($"ERROR - file already exists: {tsvPath}");

            using TsvBuilder database = new(tsvPath);
            for (int i = 0; i < abfPaths.Length; i++)
            {
                Console.WriteLine($"READING [{i + 1:N0} of {abfPaths.Length:N0}] {abfPaths[i]}");
                database.Add(abfPaths[i]);
            }
        }

        /// <summary>
        /// Create (or extend) a SQL database given a TSV file of ABF file info.
        /// </summary>
        public static void CreateSQL(string tsvPath, string dbPath, int limit = int.MaxValue)
        {
            if (File.Exists(dbPath))
                throw new InvalidOperationException($"ERROR - file already exists: {dbPath}");

            AbfRecord[] abfs = File.ReadLines(tsvPath)
                .Skip(1)
                .Where(x => x.Length > 10)
                .Select(x => AbfRecord.FromTSV(x))
                .ToArray();

            AbfDatabase database = new(dbPath);
            limit = Math.Min(limit, abfs.Length);
            for (int i = 0; i < limit; i++)
            {
                Console.WriteLine($"INSERTING [{i + 1:N0} of {limit:N0}] {abfs[i].FullPath}");
                database.Add(abfs[i]);
            }
        }
    }
}
