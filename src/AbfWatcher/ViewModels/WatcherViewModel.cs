using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfWatcher.ViewModels
{
    internal class WatcherViewModel : INotifyPropertyChanged
    {
        public string WatchFolder { get => Watcher.WatchFolder; set { } }
        public string DatabaseFolder { get => Watcher.DatabaseFolder; set { } }
        public int FilesTracked { get => Watcher.FilesTracked; set { } }
        public string LogText { get => "Log Text..."; set { } }

        private readonly Models.Watcher Watcher;

        internal WatcherViewModel(string watchFolder, string databaseFolder)
        {
            Watcher = new(watchFolder, databaseFolder);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
