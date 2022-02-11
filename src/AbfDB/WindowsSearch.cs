using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;

namespace AbfDB;

internal static class WindowsSearch
{
    public static Dictionary<string, AbfRecord> FindIndexedAbfs(string searchFolder)
    {
        Console.WriteLine("Searching filesystem for ABF files...");
        Dictionary<string, AbfRecord> abfs = WindowsSearch.FindAbfs(searchFolder);
        Console.WriteLine($"Located {abfs.Count:N0} ABFs in the filesystem.");
        return abfs;
    }

    public static Dictionary<string, AbfRecord> FindAbfs(string basePath)
    {
        basePath = Path.GetFullPath(basePath);
        string query = "SELECT System.ItemPathDisplay, System.DateModified, System.Size FROM SystemIndex " +
                       $"WHERE scope ='file:{basePath}' " +
                       "AND System.ItemName LIKE '%.abf'";

        using OleDbConnection connection = new(@"Provider=Search.CollatorDSO;Extended Properties='Application=Windows'");
        connection.Open();
        using OleDbCommand command = new(query, connection);
        using OleDbDataReader reader = command.ExecuteReader();

        Dictionary<string, AbfRecord> abfs = new();
        while (reader.Read())
        {
            string path = reader.GetString(0);
            DateTime modified = reader.GetDateTime(1);
            int sizeBytes = (int)reader.GetDecimal(2);

            string folder = Path.GetDirectoryName(path) ?? string.Empty;
            string rsvFileName = Path.GetFileNameWithoutExtension(path) + ".rsv";
            string rsvPath = Path.Combine(folder, rsvFileName);

            AbfRecord abf = new()
            {
                FullPath = path,
                Modified = modified,
                SizeBytes = sizeBytes,
            };

            abfs.Add(abf.FullPath, abf);
        }
        connection.Close();
        return abfs;
    }
}
