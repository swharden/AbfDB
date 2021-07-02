using System;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace AbfDB.Tables
{
    public static class Scans
    {
        public static void Create(SqliteConnection conn)
        {
            const string createTableCommandText =
                "CREATE TABLE Scans" +
                "(" +
                    "[id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                    "[folder] TEXT NOT NULL, " +
                    "[started] TEXT NOT NULL, " +
                    "[duration] TEXT NOT NULL, " +
                    "[abfs] INTEGER NOT NULL, " +
                    "[folders] INTEGER NOT NULL, " +
                    "[errors] INTEGER NOT NULL" +
                ")";

            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = createTableCommandText;
            cmd.ExecuteNonQuery();
        }

        public static void Add(SqliteConnection conn, string folder, Stopwatch sw, int abfs, int folders, int errors)
        {
            using SqliteCommand cmd = new("INSERT INTO Scans " +
                "(folder, started, duration, abfs, folders, errors) " +
                "VALUES (@folder, @started, @duration, @abfs, @folders, @errors)", conn);

            // WARNING: never insert data into SQL commands by combining strings
            cmd.Parameters.AddWithValue("folder", folder);
            cmd.Parameters.AddWithValue("started", (DateTime.Now - sw.Elapsed).ToString());
            cmd.Parameters.AddWithValue("duration", sw.Elapsed.ToString());
            cmd.Parameters.AddWithValue("abfs", abfs);
            cmd.Parameters.AddWithValue("folders", folders);
            cmd.Parameters.AddWithValue("errors", errors);

            cmd.ExecuteNonQuery();
        }
    }
}
