using System;
using Microsoft.Data.Sqlite;

namespace AbfDB.Tables
{
    public static class Errors
    {
        public static void Create(SqliteConnection conn)
        {
            const string createTableCommandText =
                "CREATE TABLE Errors" +
                "(" +
                    "[id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                    "[timestamp] TEXT NOT NULL, " +
                    "[abf_path] TEXT NOT NULL, " +
                    "[details] TEXT NOT NULL" +
                ")";

            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = createTableCommandText;
            cmd.ExecuteNonQuery();
        }

        public static void Add(SqliteConnection conn, string abfFilePath, Exception ex)
        {
            using SqliteCommand cmd = new("INSERT INTO Errors " +
                "(timestamp, abf_path, details) " +
                "VALUES (@timestamp, @abf_path, @details)", conn);

            // WARNING: never insert data into SQL commands by combining strings
            cmd.Parameters.AddWithValue("timestamp", DateTime.Now.ToString());
            cmd.Parameters.AddWithValue("abf_path", abfFilePath);
            cmd.Parameters.AddWithValue("details", ex.ToString());

            cmd.ExecuteNonQuery();
        }
    }
}
