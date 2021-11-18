using Microsoft.Data.Sqlite;

namespace AbfDB
{
    /// <summary>
    /// This class manages a database of ABF files.
    /// This is the public interface to the database (its schema is internal).
    /// </summary>
    public class AbfDatabase : IDisposable
    {
        public readonly string FilePath;
        public int Count { get; private set; }
        public readonly Queue<LogMessage> LogMessages = new();
        private readonly SqliteConnection Connection;

        public AbfDatabase(string file)
        {
            FilePath = Path.GetFullPath(file);

            SqliteConnectionStringBuilder csBuilder = new() { DataSource = FilePath };
            Connection = new(csBuilder.ConnectionString);
            Connection.Open();
        }

        public void Dispose()
        {
            Connection.Close();
        }

        private void Log(string verb, string noun)
        {
            LogMessage message = new(verb, noun);
            System.Diagnostics.Debug.WriteLine(message);
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
