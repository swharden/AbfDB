using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace AbfDB.Databases
{
    public class SqliteDatabase : AbfDatabase
    {
        private readonly SqliteConnection Connection;
        private readonly string FilePath;

        public SqliteDatabase(string filename)
        {
            FilePath = Path.GetFullPath(filename);

            if (File.Exists(FilePath))
                File.Delete(FilePath);

            Connection = new SqliteConnection($"Data Source={filename};");
            Connection.Open();

            CreateTable();
        }

        private void CreateTable()
        {
            const string createTableCommandText =
                "CREATE TABLE Abfs" +
                "(" +
                    "[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                    "[Folder] TEXT NOT NULL, " +
                    "[Filename] TEXT NOT NULL, " +
                    "[Episodes] INTEGER NOT NULL, " +
                    "[Date] INTEGER NOT NULL, " +
                    "[Time] INTEGER NOT NULL, " +
                    "[Stopwatch] INTEGER NOT NULL" +
                ")";

            using var createTableCommand = Connection.CreateCommand();
            createTableCommand.CommandText = createTableCommandText;
            createTableCommand.ExecuteNonQuery();
        }

        public override void AddAbf(string path, int episodes, uint date, uint time, int stopwatch)
        {
            using var insertAbfCommand = new SqliteCommand("INSERT INTO Abfs " +
                "(Folder, Filename, Episodes, Date, Time, Stopwatch) " +
                "VALUES (@folder, @filename, @episodes, @date, @time, @stopwatch)", Connection);

            // WARNING: never insert data into SQL commands by editing strings!
            insertAbfCommand.Parameters.AddWithValue("folder", Path.GetDirectoryName(path));
            insertAbfCommand.Parameters.AddWithValue("filename", Path.GetFileName(path));
            insertAbfCommand.Parameters.AddWithValue("episodes", episodes);
            insertAbfCommand.Parameters.AddWithValue("date", date);
            insertAbfCommand.Parameters.AddWithValue("time", time);
            insertAbfCommand.Parameters.AddWithValue("stopwatch", time);

            insertAbfCommand.ExecuteNonQuery();
        }

        public override void Dispose()
        {
            Connection.Close();
            Connection.Dispose();
            Console.WriteLine($"Wrote: {FilePath}");
        }
    }
}
