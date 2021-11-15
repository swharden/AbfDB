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
        }

        private string GetConnectionString()
        {
            var csBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = DatabaseFilePath
            };
            return csBuilder.ConnectionString;
        }

        public void UpdateCount(SqliteConnection conn)
        {
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Abfs;";
            object result = cmd.ExecuteScalar() ?? 0;
            Int64 count = (Int64)result;
            AbfCount = (int)count;
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
                    "[Guid] TEXT, " +
                    "[Created] TEXT, " +
                    "[Protocol] TEXT, " +
                    "[LengthSec] REAL, " +
                    "[Comments] TEXT" +
                ")";

            conn.Open();
            cmd.ExecuteNonQuery();
            UpdateCount(conn);
            conn.Close();

            Debug.WriteLine($"Database INITIALIZED: {DatabaseFilePath}");
        }

        public void Create(string path)
        {
            path = Path.GetFullPath(path);
            Debug.WriteLine($"Database CREATE: {path}");

            using SqliteConnection conn = new(GetConnectionString());

            using SqliteCommand cmdDelete = new("DELETE FROM Abfs WHERE Folder = @fldr AND Filename = @fn", conn);
            cmdDelete.Parameters.AddWithValue("fldr", Path.GetDirectoryName(path));
            cmdDelete.Parameters.AddWithValue("fn", Path.GetFileName(path));

            using var cmdCreate = new SqliteCommand("INSERT INTO Abfs " +
                "(Folder, Filename, Guid, Created, Protocol, LengthSec, Comments) " +
                "VALUES (@folder, @filename, @guid, @created, @protocol, @lengthSec, @comments)", conn);
            cmdCreate.Parameters.AddWithValue("folder", Path.GetDirectoryName(path));
            cmdCreate.Parameters.AddWithValue("filename", Path.GetFileName(path));
            try
            {
                AbfSharp.ABFFIO.ABF abf = new(path, preloadSweepData: false);
                cmdCreate.Parameters.AddWithValue("guid", AbfInfo.GetCjfGuid(abf));
                cmdCreate.Parameters.AddWithValue("created", AbfInfo.GetCreationDateTime(abf));
                cmdCreate.Parameters.AddWithValue("protocol", AbfInfo.GetProtocol(abf));
                cmdCreate.Parameters.AddWithValue("lengthSec", AbfInfo.GetLengthSec(abf));
                cmdCreate.Parameters.AddWithValue("comments", AbfInfo.GetCommentSummary(abf));
            }
            catch
            {
                Debug.WriteLine($"ABF HEADER ERROR: {path}");
                cmdCreate.Parameters.AddWithValue("guid", "");
                cmdCreate.Parameters.AddWithValue("created", "");
                cmdCreate.Parameters.AddWithValue("protocol", "");
                cmdCreate.Parameters.AddWithValue("lengthSec", 0);
                cmdCreate.Parameters.AddWithValue("comments", "");
            }

            conn.Open();
            cmdDelete.ExecuteNonQuery();
            cmdCreate.ExecuteNonQuery();
            UpdateCount(conn);
            conn.Close();
        }

        public void Delete(string path)
        {
            path = Path.GetFullPath(path);
            Debug.WriteLine($"Database DELETE: {path}");
            Debug.WriteLine(Path.GetDirectoryName(path));
            Debug.WriteLine(Path.GetFileName(path));

            using SqliteConnection conn = new(GetConnectionString());
            using SqliteCommand cmd = new("DELETE FROM Abfs WHERE Folder = @fldr AND Filename = @fn", conn);
            cmd.Parameters.AddWithValue("fldr", Path.GetDirectoryName(path));
            cmd.Parameters.AddWithValue("fn", Path.GetFileName(path));

            conn.Open();
            cmd.ExecuteNonQuery();
            UpdateCount(conn);
            conn.Close();
        }
    }
}
