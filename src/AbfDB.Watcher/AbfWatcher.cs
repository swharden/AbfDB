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
        public readonly Queue<LogMessage> Messages = new();

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

            AddAllFilesInFolder(folderPath);
            AddAllFilesInFolder(@"X:\Data\SD\practice\Scott\2021-11-16-AON-2P");
        }

        public void AddAllFilesInFolder(string folderPath)
        {
            string[] abfPaths = Directory.GetFiles(folderPath, "*.abf");
            foreach (string abfPath in abfPaths)
            {
                Database.Add(abfPath);
                Messages.Enqueue(new LogMessage("Manually Added", abfPath));
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Database.Add(e.FullPath);
            Messages.Enqueue(new LogMessage("Changed", e.FullPath));
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            Database.Add(e.FullPath);
            Messages.Enqueue(new LogMessage("Created", e.FullPath));
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Database.Remove(e.FullPath);
            Messages.Enqueue(new LogMessage("Deleted", e.FullPath));
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            Database.Remove(e.OldFullPath);
            Messages.Enqueue(new LogMessage("Renamed From", e.OldFullPath));

            Database.Add(e.FullPath);
            Messages.Enqueue(new LogMessage("Renamed To", e.FullPath));
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Exception? exception = e.GetException();
            string message = exception is null ? "?" : exception.Message;
            Messages.Enqueue(new LogMessage("ERROR", message));
        }
    }
}
