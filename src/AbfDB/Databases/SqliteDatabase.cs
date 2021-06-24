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
                    "[Stopwatch] INTEGER NOT NULL, " +
                    "[MD5] TEXT NOT NULL" +
                ")";

            using var createTableCommand = Connection.CreateCommand();
            createTableCommand.CommandText = createTableCommandText;
            createTableCommand.ExecuteNonQuery();
        }

        public override void Add(AbfRecord record)
        {
            using var insertAbfCommand = new SqliteCommand("INSERT INTO Abfs " +
                "(Folder, Filename, Episodes, Date, Time, Stopwatch, MD5) " +
                "VALUES (@folder, @filename, @episodes, @date, @time, @stopwatch, @md5)", Connection);

            // WARNING: never insert data into SQL commands by editing strings!
            insertAbfCommand.Parameters.AddWithValue("folder", Path.GetDirectoryName(record.FullPath));
            insertAbfCommand.Parameters.AddWithValue("filename", Path.GetFileName(record.FullPath));
            insertAbfCommand.Parameters.AddWithValue("episodes", record.Episodes);
            insertAbfCommand.Parameters.AddWithValue("date", record.Date);
            insertAbfCommand.Parameters.AddWithValue("time", record.Time);
            insertAbfCommand.Parameters.AddWithValue("stopwatch", record.Stopwatch);
            insertAbfCommand.Parameters.AddWithValue("md5", record.FileHashMD5);

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
