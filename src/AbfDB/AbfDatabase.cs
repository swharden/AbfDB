using Microsoft.Data.Sqlite;
using System;
using System.IO;

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
                    "[SizeBytes] INTEGER NOT NULL, " +
                    "[Guid] TEXT, " +
                    "[Recorded] TEXT, " +
                    "[Noted] TEXT, " +
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
                Add(AbfRecord.FromFile(abfPath));
        }

        public void Add(string abfPath)
        {
            AbfRecord abf = AbfRecord.FromFile(abfPath);
            Add(abf);
        }

        public void Add(AbfRecord abf)
        {
            using var cmdCreate = new SqliteCommand("INSERT INTO Abfs " +
                "(Folder, Filename, SizeBytes, Guid, Recorded, Noted, Protocol, LengthSec, Comments) " +
                "VALUES (@folder, @filename, @sizeBytes, @guid, @recorded, @noted, @protocol, @lengthSec, @comments)", Connection);

            cmdCreate.Parameters.AddWithValue("folder", abf.Folder);
            cmdCreate.Parameters.AddWithValue("filename", abf.Filename);
            cmdCreate.Parameters.AddWithValue("sizeBytes", abf.SizeBytes);
            cmdCreate.Parameters.AddWithValue("guid", abf.Guid);
            cmdCreate.Parameters.AddWithValue("recorded", abf.Recorded);
            cmdCreate.Parameters.AddWithValue("noted", abf.Noted);
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
