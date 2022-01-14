using Microsoft.Data.Sqlite;
using System;
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
        SqliteConnection conn = new(ConnectionString);
        conn.Open();

        TableBuilder builder = new("Abfs");
        builder.AddColumn("ID", ColumnType.INTEGER, "NOT NULL PRIMARY KEY AUTOINCREMENT");
        builder.AddColumn("Folder", ColumnType.TEXT);
        builder.AddColumn("FileName", ColumnType.TEXT);
        builder.AddColumn("SizeBytes", ColumnType.INTEGER);
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
        SqliteConnection conn = new(ConnectionString);
        conn.Open();

        using var transaction = conn.BeginTransaction();

        var command = conn.CreateCommand();
        command.CommandText = "INSERT INTO Abfs " +
            "(Folder, FileName, SizeBytes, Protocol, Comments, RecordedTimestamp, LoggedTimestamp, RecordedDay, LoggedDay, LengthSec, Guid) " +
            "VALUES ($folder, $fileName, $sizeBytes, $protocol, $comments, $recordedTimestamp, $loggedTimestamp, $recordedDay, $loggedDay, $lengthSec, $guid)";

        SqliteParameter folderParam = command.CreateParameter();
        SqliteParameter fileNameParam = command.CreateParameter();
        SqliteParameter sizeBytesParam = command.CreateParameter();
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
            protocolParam.Value = abf.Protocol;
            commentsPram.Value = abf.Comments;
            recordedTimestampParam.Value = abf.Recorded;
            recordedDayParam.Value = abf.RecordedDay;
            loggedTimestampParam.Value = abf.Logged;
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
        abfPath = Path.GetFullPath(abfPath);

        SqliteConnection conn = new(ConnectionString);
        using SqliteCommand cmd = new("DELETE FROM Abfs WHERE Folder = @fldr AND Filename = @fn", conn);
        cmd.Parameters.AddWithValue("fldr", Path.GetDirectoryName(abfPath));
        cmd.Parameters.AddWithValue("fn", Path.GetFileName(abfPath));

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();
    }

    public int GetRecordCount()
    {
        SqliteConnection conn = new(ConnectionString);
        using SqliteCommand cmd = new("SELECT COUNT(*) FROM Abfs", conn);

        conn.Open();
        int count = Convert.ToInt32(cmd.ExecuteScalar());
        conn.Close();

        return count;
    }

    /*
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
    */
}