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
        /// Scan a folder tree and generate a new database from scratch
        /// </summary>
        public static void CreateTSV(string searchPath, string databasePath)
        {
            if (File.Exists(databasePath))
            {
                Console.WriteLine("ERROR: database file already exists.");
                return;
            };

            using TsvBuilder database = new(databasePath);
            DirectoryInfo rootFolder = new(searchPath);
            Stopwatch watch = Stopwatch.StartNew();

            Console.WriteLine("Scanning for ABF paths...");
            string[] abfPaths = FindFiles(rootFolder);

            for (int i = 0; i < abfPaths.Length; i++)
            {
                Console.WriteLine($"[{i + 1} of {abfPaths.Length}] {abfPaths[i]}");
                database.Add(abfPaths[i]);
            }

            Console.WriteLine($"Finished in: {watch.Elapsed}");
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
    }
}
