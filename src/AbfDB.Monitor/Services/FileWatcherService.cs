using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AbfDB.Monitor.Services
{
    public class FileWatcherService : IDisposable
    {
        public string Path { get; private set; }
        public string DatabaseFile { get; private set; }
        public ObservableCollection<Models.AbfFile> AbfFiles { get; private set; }

        private readonly FileSystemWatcher Watcher;
        private readonly AbfDatabase Database;

        public FileWatcherService(string path, string dbFile)
        {
            Path = System.IO.Path.GetFullPath(path);
            DatabaseFile = System.IO.Path.GetFullPath(dbFile);

            Database = new(DatabaseFile);
            AbfFiles = new();

            Watcher = new(Path)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
                Filter = "*.abf",
            };

            Watcher.Changed += OnChanged;
            Watcher.Created += OnCreated;
            Watcher.Deleted += OnDeleted;
            Watcher.Renamed += OnRenamed;
        }

        /// <summary>
        /// Process ABFs older than the given <paramref name="settleTime"/> (seconds)
        /// by adding them to the database then removing them from the tracked file list.
        /// </summary>
        public void ProcessABFs(int settleTime = 30)
        {
            Models.AbfFile[] abfsToProcess = AbfFiles.Where(x => x.Age > settleTime).ToArray();

            foreach (var abf in abfsToProcess)
            {
                Database.ProcessAbf(abf.Path);
                AbfFiles.Remove(abf);
            }
        }

        public void AddRandomFile()
        {
            AddFile(path: $"ExamplePath{AbfFiles.Count + 1}", reason: "testing");
        }

        public void AddFile(string path, string reason)
        {
            // can't edit collections directly from the UI thread
            App.Current.Dispatcher.Invoke(delegate
            {
                Models.AbfFile[] existingAbfs = AbfFiles.Where(x => x.Path == path).ToArray();

                if (existingAbfs.Any())
                {
                    foreach (var existingAbf in existingAbfs)
                        existingAbf.ResetTime();
                }
                else
                {
                    Models.AbfFile abf = new(path, reason);
                    AbfFiles.Add(abf);
                }
            });
        }

        public void Dispose()
        {
            Watcher.Dispose();
            Database.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            AddFile(path: e.FullPath, reason: "changed");
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            AddFile(path: e.FullPath, reason: "created");
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            AddFile(path: e.FullPath, reason: "deleted");
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            AddFile(path: e.OldFullPath, reason: "rename from");
            AddFile(path: e.FullPath, reason: "rename to");
        }
    }
}
