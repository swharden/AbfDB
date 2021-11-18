using Microsoft.Data.Sqlite;

namespace AbfDB
{
    /// <summary>
    /// This class encapsulates all methods that depend on a specific database schema.
    /// </summary>
    internal class AbfDatabaseAction
    {
        internal static void Initialize(SqliteConnection conn)
        {
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

            cmd.ExecuteNonQuery();
        }

        internal static void Insert(SqliteConnection conn, string abfPath)
        {
            abfPath = Path.GetFullPath(abfPath);

            using var cmdCreate = new SqliteCommand("INSERT INTO Abfs " +
                "(Folder, Filename, Guid, Created, Protocol, LengthSec, Comments) " +
                "VALUES (@folder, @filename, @guid, @created, @protocol, @lengthSec, @comments)", conn);
            cmdCreate.Parameters.AddWithValue("folder", Path.GetDirectoryName(abfPath));
            cmdCreate.Parameters.AddWithValue("filename", Path.GetFileName(abfPath));
            try
            {
                AbfSharp.ABFFIO.ABF abf = new(abfPath, preloadSweepData: false);
                cmdCreate.Parameters.AddWithValue("guid", AbfInfo.GetCjfGuid(abf));
                cmdCreate.Parameters.AddWithValue("created", AbfInfo.GetCreationDateTime(abf));
                cmdCreate.Parameters.AddWithValue("protocol", AbfInfo.GetProtocol(abf));
                cmdCreate.Parameters.AddWithValue("lengthSec", AbfInfo.GetLengthSec(abf));
                cmdCreate.Parameters.AddWithValue("comments", AbfInfo.GetCommentSummary(abf));
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"ABF HEADER ERROR: {abfPath}");
                cmdCreate.Parameters.AddWithValue("guid", "");
                cmdCreate.Parameters.AddWithValue("created", "");
                cmdCreate.Parameters.AddWithValue("protocol", "");
                cmdCreate.Parameters.AddWithValue("lengthSec", 0);
                cmdCreate.Parameters.AddWithValue("comments", "");
            }

            cmdCreate.ExecuteNonQuery();
        }

        internal static void Delete(SqliteConnection conn, string abfPath)
        {
            abfPath = Path.GetFullPath(abfPath);

            using SqliteCommand cmd = new("DELETE FROM Abfs WHERE Folder = @fldr AND Filename = @fn", conn);
            cmd.Parameters.AddWithValue("fldr", Path.GetDirectoryName(abfPath));
            cmd.Parameters.AddWithValue("fn", Path.GetFileName(abfPath));

            cmd.ExecuteNonQuery();
        }
    }
}
