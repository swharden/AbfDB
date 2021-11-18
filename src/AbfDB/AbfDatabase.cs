using Microsoft.Data.Sqlite;

namespace AbfDB
{
    public class AbfDatabase : IDisposable
    {
        public readonly string FilePath;
        public int Count { get; private set; }
        private readonly SqliteConnection Connection;

        public AbfDatabase(string file)
        {
            FilePath = Path.GetFullPath(file);

            SqliteConnectionStringBuilder csBuilder = new() { DataSource = FilePath };
            Connection = new(csBuilder.ConnectionString);
            Connection.Open();
            Initialize();
        }

        public void Dispose()
        {
            Connection.Close();
        }

        private void Initialize()
        {
            using SqliteCommand cmd = Connection.CreateCommand();
            cmd.CommandText =
                "CREATE TABLE IF NOT EXISTS Abfs" +
                "(" +
                    "[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                    "[Folder] TEXT NOT NULL, " +
                    "[Filename] TEXT NOT NULL, " +
                    "[Guid] TEXT, " +
                    "[Created] TEXT, " +
                    "[Protocol] TEXT, " +
                    "[LengthSec] REAL, " +
                    "[Comments] TEXT" +
                ")";

            cmd.ExecuteNonQuery();
        }

        public void Add(string abfPath, bool replace = true)
        {
            // TODO: skip incomplete files (with .RST or something like that)
            abfPath = Path.GetFullPath(abfPath);
            string folder = Path.GetDirectoryName(abfPath) ?? "";
            string filename = Path.GetFileName(abfPath);

            try
            {
                AbfSharp.ABFFIO.ABF abf = new(abfPath, preloadSweepData: false);

                if (replace)
                    Remove(abfPath);

                Add(folder, 
                    filename,
                    guid: AbfInfo.GetCjfGuid(abf),
                    created: AbfInfo.GetCreationDateTime(abf),
                    protocol: AbfInfo.GetProtocol(abf),
                    lengthSec: AbfInfo.GetLengthSec(abf),
                    comments: AbfInfo.GetCommentSummary(abf));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ABF HEADER ERROR: {abfPath}");
                Add(folder,
                    filename,
                    guid: "",
                    created: DateTime.Now,
                    protocol: "EXCEPTION",
                    lengthSec: -1,
                    comments: ex.Message);
            }
        }

        public void Add(string folder, string filename, string guid, DateTime created, string protocol, double lengthSec, string comments)
        {

            using var cmdCreate = new SqliteCommand("INSERT INTO Abfs " +
                "(Folder, Filename, Guid, Created, Protocol, LengthSec, Comments) " +
                "VALUES (@folder, @filename, @guid, @created, @protocol, @lengthSec, @comments)", Connection);

            cmdCreate.Parameters.AddWithValue("folder", folder);
            cmdCreate.Parameters.AddWithValue("filename", filename);
            cmdCreate.Parameters.AddWithValue("guid", guid);
            cmdCreate.Parameters.AddWithValue("created", created);
            cmdCreate.Parameters.AddWithValue("protocol", protocol);
            cmdCreate.Parameters.AddWithValue("lengthSec", lengthSec);
            cmdCreate.Parameters.AddWithValue("comments", comments);

            cmdCreate.ExecuteNonQuery();
        }

        public void Remove(string abfPath)
        {
            abfPath = Path.GetFullPath(abfPath);

            using SqliteCommand cmd = new("DELETE FROM Abfs WHERE Folder = @fldr AND Filename = @fn", Connection);
            cmd.Parameters.AddWithValue("fldr", Path.GetDirectoryName(abfPath));
            cmd.Parameters.AddWithValue("fn", Path.GetFileName(abfPath));

            cmd.ExecuteNonQuery();
        }
    }
}
