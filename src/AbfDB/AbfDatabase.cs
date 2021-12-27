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
            GC.SuppressFinalize(this);
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

        /// <summary>
        /// Delete all existing records of this ABF from the database,
        /// then if the file exists on disk read it and add the record to the database.
        /// </summary>
        /// <param name="abfPath"></param>
        public void ProcessAbf(string abfPath)
        {
            Remove(abfPath);
            if (File.Exists(abfPath))
                Add(abfPath);
        }

        private static bool AbfHasRsvFile(string abfPath)
        {
            abfPath = Path.GetFullPath(abfPath);
            string abfID = Path.GetFileNameWithoutExtension(abfPath);
            string abfFolder = Path.GetDirectoryName(abfPath) ?? string.Empty;
            string rsvFilePath = Path.Combine(abfFolder, abfID + ".rsv");
            return File.Exists(rsvFilePath);
        }

        public void Add(string abfPath)
        {
            AbfRecord abfRecord = new();

            abfPath = Path.GetFullPath(abfPath);
            abfRecord.Folder = Path.GetDirectoryName(abfPath) ?? string.Empty;
            abfRecord.Filename = Path.GetFileName(abfPath);
            abfRecord.Created = DateTime.Now;

            try
            {
                AbfSharp.ABFFIO.ABF abf = new(abfPath, preloadSweepData: false);
                abfRecord.Guid = AbfInfo.GetCjfGuid(abf);
                abfRecord.Created = AbfInfo.GetCreationDateTime(abf);
                abfRecord.Protocol = AbfInfo.GetProtocol(abf);
                abfRecord.LengthSec = AbfInfo.GetLengthSec(abf);
                abfRecord.Comments = AbfInfo.GetCommentSummary(abf);
            }
            catch (Exception ex)
            {
                if (AbfHasRsvFile(abfPath))
                {
                    System.Diagnostics.Debug.WriteLine($"INCOMPLETE ABF: {abfPath}");
                    abfRecord.Protocol = "INCOMPLETE";
                    abfRecord.Comments = "has RSV file";
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"ABF HEADER ERROR: {abfPath}");
                    abfRecord.Protocol = "EXCEPTION";
                    abfRecord.Comments = ex.Message;
                }
            }

            Add(abfRecord);
        }

        public void Add(AbfRecord abf)
        {
            using var cmdCreate = new SqliteCommand("INSERT INTO Abfs " +
                "(Folder, Filename, Guid, Created, Protocol, LengthSec, Comments) " +
                "VALUES (@folder, @filename, @guid, @created, @protocol, @lengthSec, @comments)", Connection);

            cmdCreate.Parameters.AddWithValue("folder", abf.Folder);
            cmdCreate.Parameters.AddWithValue("filename", abf.Filename);
            cmdCreate.Parameters.AddWithValue("guid", abf.Guid);
            cmdCreate.Parameters.AddWithValue("created", abf.Created);
            cmdCreate.Parameters.AddWithValue("protocol", abf.Protocol);
            cmdCreate.Parameters.AddWithValue("lengthSec", abf.LengthSec);
            cmdCreate.Parameters.AddWithValue("comments", abf.Comments);

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

        public void UpdateCount()
        {
            using SqliteCommand cmd = new("SELECT COUNT(*) FROM Abfs", Connection);
            Count = Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
