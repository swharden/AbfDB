using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AbfDB.Database;

public class AbfDatabase
{
    readonly string ConnectionString;

    public AbfDatabase(string filePath)
    {
        SqliteConnectionStringBuilder csBuilder = new() { DataSource = Path.GetFullPath(filePath) };
        ConnectionString = csBuilder.ConnectionString;
        CreateTableIfNotExist();
    }

    private void CreateTableIfNotExist()
    {
        using SqliteConnection conn = new(ConnectionString);
        conn.Open();

        TableBuilder builder = new("Abfs");
        builder.AddColumn("ID", ColumnType.INTEGER, "NOT NULL PRIMARY KEY AUTOINCREMENT");
        builder.AddColumn("Folder", ColumnType.TEXT);
        builder.AddColumn("FileName", ColumnType.TEXT);
        builder.AddColumn("SizeBytes", ColumnType.INTEGER);
        builder.AddColumn("ModifiedTimestamp", ColumnType.TEXT);
        builder.AddColumn("Protocol", ColumnType.TEXT);
        builder.AddColumn("Comments", ColumnType.TEXT);
        builder.AddColumn("RecordedTimestamp", ColumnType.TEXT);
        builder.AddColumn("LoggedTimestamp", ColumnType.TEXT);
        builder.AddColumn("RecordedDay", ColumnType.INTEGER);
        builder.AddColumn("LoggedDay", ColumnType.INTEGER);
        builder.AddColumn("LengthSec", ColumnType.NUMERIC);
        builder.AddColumn("Guid", ColumnType.TEXT);

        using SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = builder.ToString();
        cmd.ExecuteNonQuery();

        conn.Close();
    }

    public void Add(string abfPath)
    {
        AbfRecord abf = AbfFile.GetRecord(abfPath);
        Add(abf);
    }

    public void Add(string[] abfPaths)
    {
        AbfRecord[] abfs = abfPaths.Select(x => AbfFile.GetRecord(x)).ToArray();
        Add(abfs);
    }

    public void Add(AbfRecord abf)
    {
        AbfRecord[] abfs = { abf };
        Add(abfs);
    }

    public void Add(AbfRecord[] abfs)
    {
        using SqliteConnection conn = new(ConnectionString);
        conn.Open();

        using var transaction = conn.BeginTransaction();

        var command = conn.CreateCommand();
        command.CommandText = "INSERT INTO Abfs " +
            "(Folder, FileName, SizeBytes, ModifiedTimestamp, Protocol, Comments, RecordedTimestamp, LoggedTimestamp, RecordedDay, LoggedDay, LengthSec, Guid) " +
            "VALUES ($folder, $fileName, $sizeBytes, $modified, $protocol, $comments, $recordedTimestamp, $loggedTimestamp, $recordedDay, $loggedDay, $lengthSec, $guid)";

        SqliteParameter folderParam = command.CreateParameter();
        SqliteParameter fileNameParam = command.CreateParameter();
        SqliteParameter sizeBytesParam = command.CreateParameter();
        SqliteParameter modifiedParam = command.CreateParameter();
        SqliteParameter protocolParam = command.CreateParameter();
        SqliteParameter commentsPram = command.CreateParameter();
        SqliteParameter recordedTimestampParam = command.CreateParameter();
        SqliteParameter loggedTimestampParam = command.CreateParameter();
        SqliteParameter recordedDayParam = command.CreateParameter();
        SqliteParameter loggedDayParam = command.CreateParameter();
        SqliteParameter lengthSecParam = command.CreateParameter();
        SqliteParameter guidParam = command.CreateParameter();

        folderParam.ParameterName = "$folder";
        fileNameParam.ParameterName = "$fileName";
        sizeBytesParam.ParameterName = "$sizeBytes";
        modifiedParam.ParameterName = "$modified";
        protocolParam.ParameterName = "$protocol";
        commentsPram.ParameterName = "$comments";
        recordedTimestampParam.ParameterName = "$recordedTimestamp";
        loggedTimestampParam.ParameterName = "$loggedTimestamp";
        recordedDayParam.ParameterName = "$recordedDay";
        loggedDayParam.ParameterName = "$loggedDay";
        lengthSecParam.ParameterName = "$lengthSec";
        guidParam.ParameterName = "$guid";

        command.Parameters.Add(folderParam);
        command.Parameters.Add(fileNameParam);
        command.Parameters.Add(sizeBytesParam);
        command.Parameters.Add(modifiedParam);
        command.Parameters.Add(protocolParam);
        command.Parameters.Add(commentsPram);
        command.Parameters.Add(recordedTimestampParam);
        command.Parameters.Add(loggedTimestampParam);
        command.Parameters.Add(recordedDayParam);
        command.Parameters.Add(loggedDayParam);
        command.Parameters.Add(lengthSecParam);
        command.Parameters.Add(guidParam);

        foreach (AbfRecord abf in abfs)
        {
            folderParam.Value = abf.Folder;
            fileNameParam.Value = abf.Filename;
            sizeBytesParam.Value = abf.SizeBytes;
            modifiedParam.Value = abf.Modified.ToString(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern);
            protocolParam.Value = abf.Protocol;
            commentsPram.Value = abf.Comments;
            recordedTimestampParam.Value = abf.Recorded.ToString(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern);
            recordedDayParam.Value = abf.RecordedDay;
            loggedTimestampParam.Value = abf.Logged.ToString(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern);
            loggedDayParam.Value = abf.LoggedDay;
            lengthSecParam.Value = abf.LengthSec;
            guidParam.Value = abf.Guid;

            command.ExecuteNonQuery();
        }

        transaction.Commit();
        conn.Close();
    }

    public void Remove(string abfPath)
    {
        Remove(new string[] { abfPath });
    }

    public void Remove(string[] abfPaths)
    {
        using SqliteConnection conn = new(ConnectionString);
        conn.Open();

        using var transaction = conn.BeginTransaction();

        var command = conn.CreateCommand();
        command.CommandText = "DELETE FROM Abfs WHERE Folder = @folder AND Filename = @filename";
        SqliteParameter folderParam = command.Parameters.AddWithValue("@folder", string.Empty);
        SqliteParameter fileNameParam = command.Parameters.AddWithValue("@filename", string.Empty);

        foreach (string abfPath in abfPaths)
        {
            folderParam.Value = Path.GetDirectoryName(abfPath);
            fileNameParam.Value = Path.GetFileName(abfPath);
            command.ExecuteNonQuery();
        }

        transaction.Commit();
        conn.Close();
    }

    public int GetRecordCount()
    {
        using SqliteConnection conn = new(ConnectionString);
        using SqliteCommand cmd = new("SELECT COUNT(*) FROM Abfs", conn);

        conn.Open();
        int count = Convert.ToInt32(cmd.ExecuteScalar());
        conn.Close();

        return count;
    }

    public AbfRecord[] GetIndexedAbfs()
    {
        Console.WriteLine("Searching database for ABF files...");

        using SqliteConnection conn = new(ConnectionString);

        string query = $"SELECT Folder, Filename, ModifiedTimestamp, SizeBytes FROM Abfs";
        using SqliteCommand cmd = new(query, conn);

        conn.Open();

        SqliteDataReader reader = cmd.ExecuteReader();

        List<AbfRecord> abfs = new();
        HashSet<string> seenPaths = new();

        while (reader.Read())
        {
            string filename = reader["Filename"].ToString() ?? string.Empty;
            string folder = reader["Folder"].ToString() ?? string.Empty;
            string path = Path.Combine(folder, filename);
            DateTime modified = DateTime.Parse(reader["ModifiedTimestamp"].ToString() ?? string.Empty);
            int sizeBytes = int.Parse(reader["SizeBytes"].ToString() ?? string.Empty);

            if (seenPaths.Contains(path))
            {
                Console.WriteLine($"SKIPPING DUPLICATE: {path}");
            }
            else
            {
                AbfRecord abf = new() { FullPath = path, Modified = modified, SizeBytes = sizeBytes };
                abfs.Add(abf);
                seenPaths.Add(path);
            }
        }

        conn.Close();

        Console.WriteLine($"Located {abfs.Count:N0} ABFs in the database.");

        return abfs.ToArray();
    }
}