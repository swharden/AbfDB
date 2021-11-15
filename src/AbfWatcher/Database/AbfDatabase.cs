using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbfWatcher.Models;
using Microsoft.Data.Sqlite;

namespace AbfWatcher.Database
{
    internal class AbfDatabase
    {
        public int AbfCount { get; private set; }
        public readonly string DatabaseFilePath;

        public AbfDatabase(string databaseFolder)
        {
            databaseFolder = Path.GetFullPath(databaseFolder);
            if (!Directory.Exists(databaseFolder))
                throw new DirectoryNotFoundException(databaseFolder);
            DatabaseFilePath = Path.Combine(databaseFolder, "abfs.db");

            Initialize();
            UpdateCount();
        }

        private string GetConnectionString()
        {
            var csBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = DatabaseFilePath
            };
            return csBuilder.ConnectionString;
        }

        public void UpdateCount()
        {
            using SqliteConnection conn = new(GetConnectionString());
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Abfs;";

            conn.Open();
            object result = cmd.ExecuteScalar() ?? 0;
            Int64 count = (Int64)result;
            AbfCount = (int)count;
            conn.Close();

            Debug.WriteLine($"Database COUNT: {AbfCount}");
        }

        public void Initialize()
        {
            using SqliteConnection conn = new(GetConnectionString());
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText =
                "CREATE TABLE IF NOT EXISTS Abfs" +
                "(" +
                    "[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                    "[Folder] TEXT NOT NULL, " +
                    "[Filename] TEXT NOT NULL, " +
                    "[Guid] TEXT NOT NULL, " +
                    "[Created] TEXT NOT NULL, " +
                    "[Protocol] TEXT, " +
                    "[LengthSec] REAL NOT NULL, " +
                    "[Comments] TEXT" +
                ")";

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            Debug.WriteLine($"Database INITIALIZED: {DatabaseFilePath}");
        }

        public void Create(string path)
        {
            path = Path.GetFullPath(path);
            Debug.WriteLine($"Database CREATE: {path}");

            using SqliteConnection conn = new(GetConnectionString());
            using var cmd = new SqliteCommand("INSERT INTO Abfs " +
                "(Folder, Filename, Guid, Created, Protocol, LengthSec, Comments) " +
                "VALUES (@folder, @filename, @guid, @created, @protocol, @lengthSec, @comments)", conn);

            cmd.Parameters.AddWithValue("folder", Path.GetDirectoryName(path));
            cmd.Parameters.AddWithValue("filename", Path.GetFileName(path));

            try
            {
                AbfSharp.ABFFIO.ABF abf = new(path, preloadSweepData: false);
                cmd.Parameters.AddWithValue("guid", AbfInfo.GetCjfGuid(abf));
                cmd.Parameters.AddWithValue("created", AbfInfo.GetCreationDateTime(abf));
                cmd.Parameters.AddWithValue("protocol", AbfInfo.GetProtocol(abf));
                cmd.Parameters.AddWithValue("lengthSec", AbfInfo.GetLengthSec(abf));
                cmd.Parameters.AddWithValue("comments", AbfInfo.GetCommentSummary(abf));
            }
            catch
            {
                Debug.WriteLine($"ABF HEADER ERROR: {path}");
            }

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            UpdateCount();
        }

        public void Read(string path)
        {
            Debug.WriteLine($"Database READ: {path}");
        }

        public void Update(string path)
        {
            Debug.WriteLine($"Database UPDATE: {path}");
        }

        public void Delete(string path)
        {
            Debug.WriteLine($"Database DELETE: {path}");
        }
    }
}
