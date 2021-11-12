﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace AbfWatcher.Models
{
    internal class Watcher
    {
        public readonly string WatchFolder;
        public string DatabaseFolder => Database.DatabaseFolder;
        public int FilesTracked => Database.AbfCount;

        private readonly FileSystemWatcher FSWatcher;
        private readonly Database.AbfDatabase Database;

        private const int MaxLogLines = 100;
        private readonly Queue<string> LogLines = new();

        public EventHandler? LogLineAdded;

        internal Watcher(string watchFolder, string databaseFolder)
        {
            watchFolder = Path.GetFullPath(watchFolder);
            if (!Directory.Exists(watchFolder))
                throw new DirectoryNotFoundException(watchFolder);
            WatchFolder = watchFolder;

            Database = new(databaseFolder);

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

        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString();
            message = $"[{timestamp}] {message}";

            LogLines.Enqueue(message);
            while (LogLines.Count > MaxLogLines)
                LogLines.Dequeue();

            LogLineAdded?.Invoke(this, EventArgs.Empty);
        }

        public string[] GetLogLines() => LogLines.ToArray();

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Log($"Changed: {e.FullPath}");
            Database.Update(e.FullPath);
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            Log($"Created: {e.FullPath}");
            Database.Create(e.FullPath);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Log($"Deleted: {e.FullPath}");
            Database.Delete(e.FullPath);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            Log($"Moved: {e.FullPath}");
            Log($" From: {e.OldFullPath}");
            Database.Delete(e.OldFullPath);
            Database.Create(e.FullPath);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            PrintException(e.GetException());
        }

        private void PrintException(Exception? ex)
        {
            if (ex is not null)
            {
                Log($"Message: {ex.Message}");
                if (ex.StackTrace is not null)
                {
                    Log("Stacktrace:");
                    Log(ex.StackTrace);
                    PrintException(ex.InnerException);
                }
            }
        }
    }
}
