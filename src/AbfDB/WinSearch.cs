using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB
{
    public static class WinSearch
    {
        public static Dictionary<string, IndexedAbf> GetIndexedAbfs(string basePath)
        {
            HashSet<string> indexedRsvPaths = WinSearch.FindIndexedFilePaths(basePath, ".rsv");

            basePath = Path.GetFullPath(basePath);
            string query = "SELECT System.ItemPathDisplay, System.DateModified, System.Size FROM SystemIndex " +
                           $"WHERE scope ='file:{basePath}' " +
                           "AND System.ItemName LIKE '%.abf'";

            using OleDbConnection connection = new(@"Provider=Search.CollatorDSO;Extended Properties='Application=Windows'");
            connection.Open();
            using OleDbCommand command = new(query, connection);
            using OleDbDataReader reader = command.ExecuteReader();

            Dictionary<string, IndexedAbf> abfs = new();
            while (reader.Read())
            {
                string path = reader.GetString(0);
                DateTime modified = reader.GetDateTime(1);
                int sizeBytes = (int)reader.GetDecimal(2);

                string folder = Path.GetDirectoryName(path) ?? string.Empty;
                string rsvFileName = Path.GetFileNameWithoutExtension(path) + ".rsv";
                string rsvPath = Path.Combine(folder, rsvFileName);

                IndexedAbf abf = new()
                {
                    Path = path,
                    Modified = modified,
                    SizeBytes = sizeBytes,
                    HasRSV = indexedRsvPaths.Contains(rsvPath),
                };

                abfs.Add(abf.Path, abf);
            }
            connection.Close();
            return abfs;
        }

        /// <summary>
        /// Returns a set of full paths for all files in the given base path ending with the given extension
        /// </summary>
        private static HashSet<string> FindIndexedFilePaths(string basePath, string extension)
        {
            basePath = System.IO.Path.GetFullPath(basePath);
            extension = extension.Replace("*", "%");
            string query = "SELECT System.ItemPathDisplay, System.DateModified, System.Size FROM SystemIndex " +
                           $"WHERE scope ='file:{basePath}' " +
                           $"AND System.ItemName LIKE '{extension}'";

            using OleDbConnection connection = new(@"Provider=Search.CollatorDSO;Extended Properties='Application=Windows'");
            connection.Open();
            using OleDbCommand command = new(query, connection);
            using OleDbDataReader reader = command.ExecuteReader();

            HashSet<string> paths = new();
            while (reader.Read())
                paths.Add(reader.GetString(0));
            connection.Close();
            return paths;
        }
    }
}
