using System;
using System.Diagnostics;

namespace AbfDB.Watcher
{
    /// <summary>
    /// This class continuously watches a directory and updates a database
    /// when ABF files are created, changed, or deleted.
    /// </summary>
    internal class AbfWatcher
    {
        private readonly AbfDatabase Database;
        public readonly string WatchFolder;
        private readonly FileSystemWatcher Watcher;

        internal AbfWatcher(string folderPath, AbfDatabase database)
        {
            Database = database;
            WatchFolder = folderPath;

            Watcher = new(folderPath)
            {
                Filter = "*.abf",
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
            };

            Watcher.NotifyFilter = NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Size;

            Watcher.Changed += OnChanged;
            Watcher.Created += OnCreated;
            Watcher.Deleted += OnDeleted;
            Watcher.Renamed += OnRenamed;
            Watcher.Error += OnError;
        }

        public void AddAllFilesInFolder(string folderPath)
        {
            string[] abfPaths = Directory.GetFiles(folderPath, "*.abf");
            foreach (string abfPath in abfPaths)
            {
                Database.Add(abfPath);
                // TODO: skip incomplete files (with .RST or something like that)
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Database.Add(e.FullPath);
            // TODO: skip incomplete files (with .RST or something like that)
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            Database.Add(e.FullPath);
            // TODO: skip incomplete files (with .RST or something like that)
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Database.Remove(e.FullPath);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            Database.Remove(e.OldFullPath);
            Database.Add(e.FullPath);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            PrintException(e.GetException());
        }

        private void PrintException(Exception? ex)
        {
            if (ex is null)
                return;

            Debug.WriteLine($"Message: {ex.Message}");
            Debug.WriteLine("Stacktrace:");
            Debug.WriteLine(ex.StackTrace);
            PrintException(ex.InnerException);
        }
    }
}
