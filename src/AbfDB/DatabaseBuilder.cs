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
        /// <summary>
        /// Scan a folder tree and generate a new database from scratch.
        /// Recording data in a TSV file greatly improves speed over logging in a SQL database.
        /// The file buffer grows in memory and is only occasionally flushed/written.
        /// In contrast the SQL database chuggs the hard disk every time a record is inserted.
        /// </summary>
        public static void CreateTSV(string searchPath, string tsvPath)
        {
            if (File.Exists(tsvPath))
            {
                Console.WriteLine($"ERROR - file already exists: {tsvPath}");
                return;
            };

            using TsvBuilder database = new(tsvPath);
            DirectoryInfo rootFolder = new(searchPath);
            Stopwatch watch = Stopwatch.StartNew();

            Console.WriteLine("Scanning for ABF paths...");
            string[] abfPaths = FindFiles(rootFolder);

            for (int i = 0; i < abfPaths.Length; i++)
            {
                Console.WriteLine($"SCANNING [{i + 1:N0} of {abfPaths.Length:N0}] {abfPaths[i]}");
                database.Add(abfPaths[i]);
            }

            Console.WriteLine($"Finished in: {watch.Elapsed} ({abfPaths.Length / watch.Elapsed.TotalSeconds:0.00} ABFs/sec)");
        }

        private static string[] FindFiles(DirectoryInfo root, string searchPattern = "*.abf")
        {
            var mdFilePaths = new List<string>();

            string[] files = Directory
                .GetFiles(root.FullName, searchPattern)
                .Where(x => x.EndsWith(".abf", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            mdFilePaths.AddRange(files);

            foreach (DirectoryInfo dir in root.GetDirectories())
                mdFilePaths.AddRange(FindFiles(dir));

            return mdFilePaths.ToArray();
        }

        /// <summary>
        /// Create (or extend) a SQL database given a TSV file of ABF file info.
        /// </summary>
        public static void CreateSQL(string tsvPath, string dbPath, int limit = int.MaxValue)
        {
            if (File.Exists(dbPath))
            {
                Console.WriteLine($"ERROR - file already exists: {dbPath}");
                return;
            };

            Console.WriteLine("Reading TSV...");
            AbfRecord[] abfs = File.ReadLines(tsvPath)
                .Skip(1)
                .Where(x => x.Length > 10)
                .Select(x => AbfRecord.FromTSV(x))
                .ToArray();

            AbfDatabase database = new(dbPath);
            Stopwatch watch = Stopwatch.StartNew();
            limit = Math.Min(limit, abfs.Length);
            for (int i = 0; i < limit; i++)
            {
                Console.WriteLine($"BUILDING [{i + 1:N0} of {limit:N0}] {abfs[i].FullPath}");
                database.Add(abfs[i]);
            }

            Console.WriteLine($"Finished in: {watch.Elapsed} ({limit / watch.Elapsed.TotalSeconds:0.00} ABFs/sec)");
        }
    }
}
