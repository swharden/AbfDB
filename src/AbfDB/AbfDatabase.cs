using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            CreateTableIfNotExist();
            UpdateCount();
        }

        public void Dispose()
        {
            Connection.Close();
            GC.SuppressFinalize(this);
        }

        private void CreateTableIfNotExist()
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
                    "[RecordedDay] NUMERIC, " +
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

        /// <summary>
        /// Insert a single record into the database.
        /// DO NOT ITERATE THIS - it is slow (opening/closing the file each time).
        /// </summary>
        public void Add(AbfRecord abf)
        {
            using var cmdCreate = new SqliteCommand("INSERT INTO Abfs " +
                "(Folder, Filename, SizeBytes, Guid, Recorded, RecordedDay, Noted, Protocol, LengthSec, Comments) " +
                "VALUES (@folder, @filename, @sizeBytes, @guid, @recorded, @recordedDay, @noted, @protocol, @lengthSec, @comments)", Connection);

            cmdCreate.Parameters.AddWithValue("folder", abf.Folder);
            cmdCreate.Parameters.AddWithValue("filename", abf.Filename);
            cmdCreate.Parameters.AddWithValue("sizeBytes", abf.SizeBytes);
            cmdCreate.Parameters.AddWithValue("guid", abf.Guid);
            cmdCreate.Parameters.AddWithValue("recorded", abf.Recorded);
            cmdCreate.Parameters.AddWithValue("recordedDay", abf.RecordedDay);
            cmdCreate.Parameters.AddWithValue("noted", abf.Noted);
            cmdCreate.Parameters.AddWithValue("protocol", abf.Protocol);
            cmdCreate.Parameters.AddWithValue("lengthSec", abf.LengthSec);
            cmdCreate.Parameters.AddWithValue("comments", abf.Comments);

            cmdCreate.ExecuteNonQuery();
        }

        /// <summary>
        /// Insert multiple ABFs into the database using bulk transactions.
        /// This overload is the most performant for inserting large numbers of ABFs.
        /// </summary>
        public void AddRange(AbfRecord[] abfs)
        {
            using var transaction = Connection.BeginTransaction();
            Console.WriteLine($"Inserting {abfs.Length:N0} ABFs...");
            var sw = System.Diagnostics.Stopwatch.StartNew();

            var command = Connection.CreateCommand();
            command.CommandText = "INSERT INTO Abfs " +
                "(Folder, Filename, SizeBytes, Guid, Recorded, RecordedDay, Noted, Protocol, LengthSec, Comments) " +
                "VALUES ($folder, $filename, $sizeBytes, $guid, $recorded, $recordedDay, $noted, $protocol, $lengthSec, $comments)";

            SqliteParameter folderParam = command.CreateParameter();
            SqliteParameter filenameParam = command.CreateParameter();
            SqliteParameter sizeBytesParam = command.CreateParameter();
            SqliteParameter guidParam = command.CreateParameter();
            SqliteParameter recordedParam = command.CreateParameter();
            SqliteParameter recordedDayParam = command.CreateParameter();
            SqliteParameter notedParam = command.CreateParameter();
            SqliteParameter protocolParam = command.CreateParameter();
            SqliteParameter lengthSecParam = command.CreateParameter();
            SqliteParameter commentsPram = command.CreateParameter();

            folderParam.ParameterName = "$folder";
            filenameParam.ParameterName = "$filename";
            sizeBytesParam.ParameterName = "$sizeBytes";
            guidParam.ParameterName = "$guid";
            recordedParam.ParameterName = "$recorded";
            recordedDayParam.ParameterName = "$recordedDay";
            notedParam.ParameterName = "$noted";
            protocolParam.ParameterName = "$protocol";
            lengthSecParam.ParameterName = "$lengthSec";
            commentsPram.ParameterName = "$comments";

            command.Parameters.Add(folderParam);
            command.Parameters.Add(filenameParam);
            command.Parameters.Add(sizeBytesParam);
            command.Parameters.Add(guidParam);
            command.Parameters.Add(recordedParam);
            command.Parameters.Add(recordedDayParam);
            command.Parameters.Add(notedParam);
            command.Parameters.Add(protocolParam);
            command.Parameters.Add(lengthSecParam);
            command.Parameters.Add(commentsPram);

            foreach (AbfRecord abf in abfs)
            {
                folderParam.Value = abf.Folder;
                filenameParam.Value = abf.Filename;
                sizeBytesParam.Value = abf.SizeBytes;
                guidParam.Value = abf.Guid;
                recordedParam.Value = abf.Recorded;
                recordedDayParam.Value = abf.RecordedDay;
                notedParam.Value = abf.Noted;
                protocolParam.Value = abf.Protocol;
                lengthSecParam.Value = abf.LengthSec;
                commentsPram.Value = abf.Comments;

                command.ExecuteNonQuery();
            }

            transaction.Commit();
            Console.WriteLine($"Transaction completed in {sw.Elapsed}");
        }

        public void AddFolder(string folderPath)
        {
            AddFolder(new DirectoryInfo(folderPath));
        }

        private void AddFolder(DirectoryInfo directory)
        {
            string[] abfPaths = Directory
                .GetFiles(directory.FullName, "*.abf")
                .Where(x => x.EndsWith(".abf", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (abfPaths.Any())
            {
                Console.WriteLine($"ADDING {abfPaths.Length} ABFs from folder: {directory}");
                AbfRecord[] abfs = abfPaths.Select(x => AbfRecord.FromFile(x)).ToArray();
                AddRange(abfs);
            }

            foreach (DirectoryInfo dir in directory.GetDirectories())
                AddFolder(dir);
        }

        public void Remove(string abfPath)
        {
            abfPath = Path.GetFullPath(abfPath);

            using SqliteCommand cmd = new("DELETE FROM Abfs WHERE Folder = @fldr AND Filename = @fn", Connection);
            cmd.Parameters.AddWithValue("fldr", Path.GetDirectoryName(abfPath));
            cmd.Parameters.AddWithValue("fn", Path.GetFileName(abfPath));

            cmd.ExecuteNonQuery();
        }

        public void RemoveFolder(string folderPath)
        {
            using SqliteCommand cmd = new("DELETE FROM Abfs WHERE Folder = @fldr", Connection);
            cmd.Parameters.AddWithValue("fldr", Path.GetFullPath(folderPath));
            cmd.ExecuteNonQuery();
        }

        public void UpdateCount()
        {
            using SqliteCommand cmd = new("SELECT COUNT(*) FROM Abfs", Connection);
            Count = Convert.ToInt32(cmd.ExecuteScalar());
        }

        public IndexedAbf[] GetIndexedAbfs()
        {
            string query = $"SELECT Folder, Filename, Recorded, SizeBytes FROM Abfs";
            using SqliteCommand cmd = new(query, Connection);
            SqliteDataReader reader = cmd.ExecuteReader();

            List<IndexedAbf> abfs = new();
            while (reader.Read())
            {
                string filename = reader["Filename"].ToString() ?? string.Empty;
                string folder = reader["Folder"].ToString() ?? string.Empty;
                string path = Path.Combine(folder, filename);

                DateTime recorded = DateTime.Parse(reader["Recorded"].ToString() ?? string.Empty);
                int sizeBytes = int.Parse(reader["SizeBytes"].ToString() ?? string.Empty);

                IndexedAbf abf = new() { Path = path, Modified = recorded, SizeBytes = sizeBytes };
                abfs.Add(abf);
            }

            return abfs.ToArray();
        }
    }
}
