using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AbfWatcher.Models
{
    internal class Watcher
    {
        public readonly string WatchFolder;
        public readonly string DatabaseFolder;
        public int FilesTracked { get; private set; } = -1;
        readonly FileSystemWatcher FSWatcher;

        internal Watcher(string watchFolder, string databaseFolder)
        {
            watchFolder = Path.GetFullPath(watchFolder);
            if (!Directory.Exists(watchFolder))
                throw new DirectoryNotFoundException(watchFolder);
            WatchFolder = watchFolder;

            databaseFolder = Path.GetFullPath(databaseFolder);
            if (!Directory.Exists(databaseFolder))
                throw new DirectoryNotFoundException(databaseFolder);
            DatabaseFolder = databaseFolder;

            FSWatcher = new FileSystemWatcher(WatchFolder)
            {
                Filter = "*.abf",
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
            };
            FSWatcher.Changed += OnChanged;
            FSWatcher.Created += OnCreated;
            FSWatcher.Deleted += OnDeleted;
            FSWatcher.Renamed += OnRenamed;
            FSWatcher.Error += OnError;
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Created: {e.FullPath}");
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Moved: {e.FullPath}");
            Console.WriteLine($" From: {e.OldFullPath}");
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}
