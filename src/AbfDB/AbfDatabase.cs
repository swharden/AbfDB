using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB
{
    /// <summary>
    /// This class manages a database of ABF files
    /// </summary>
    public class AbfDatabase
    {
        public readonly string FilePath;
        public int Count { get; private set; }
        public readonly Queue<LogMessage> LogMessages = new();

        public AbfDatabase(string file)
        {
            FilePath = Path.GetFullPath(file);
            Log("Initializing", FilePath);
        }

        private void Log(string verb, string noun)
        {
            LogMessage message = new(verb, noun);
            Debug.WriteLine(message);
            LogMessages.Enqueue(message);
        }

        public LogMessage[] GetLogMessages()
        {
            LogMessage[] messages = LogMessages.ToArray();
            LogMessages.Clear();
            return messages;
        }

        public void Remove(string path)
        {
            string folder = Path.GetDirectoryName(path) ?? "";
            string filename = Path.GetFileName(path);
            Remove(folder, filename);
        }

        public void Remove(string folder, string filename)
        {
            Count -= 1;
            string path = Path.Combine(folder, filename);
            Log("Removed", path);
        }

        public void Add(string path)
        {
            string folder = Path.GetDirectoryName(path) ?? "";
            string filename = Path.GetFileName(path);
            Add(folder, filename);
        }

        public void Add(string folder, string filename)
        {
            Count += 1;
            string path = Path.Combine(folder, filename);
            Log("Added", path);
        }
    }
}
