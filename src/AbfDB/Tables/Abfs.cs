using System;
using Microsoft.Data.Sqlite;

namespace AbfDB.Tables
{
    public static class Abfs
    {
        public static void Create(SqliteConnection conn)
        {
            const string createTableCommandText =
                "CREATE TABLE Abfs" +
                "(" +
                    "[id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                    "[folder] TEXT NOT NULL, " +
                    "[filename] TEXT NOT NULL, " +
                    "[cjf_guid] TEXT NOT NULL, " +
                    "[date] INTEGER NOT NULL, " +
                    "[protocol] TEXT NOT NULL, " +
                    "[duration_minutes] INTEGER NOT NULL, " +
                    "[tags] TEXT" +
                ")";

            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = createTableCommandText;
            cmd.ExecuteNonQuery();
        }

        public static void Add(SqliteConnection conn, string abfFilePath)
        {
            AbfSharp.ABFFIO.ABF abf = new(abfFilePath, preloadSweepData: false);

            // calculate fields that will be inserted into the database
            double sampleRate = 1e6 / abf.Header.fADCSequenceInterval / abf.Header.nADCNumChannels;
            double abfLengthSeconds = abf.Header.lActualAcqLength / sampleRate;
            double abfLengthMinutes = Math.Round(abfLengthSeconds / 60, 2);

            using SqliteCommand cmd = new("INSERT INTO Abfs " +
                "(folder, filename, cjf_guid, date, protocol, duration_minutes, tags) " +
                "VALUES (@folder, @filename, @cjf_guid, @date, @protocol, @duration_minutes, @tags)", conn);

            // WARNING: never insert data into SQL commands by combining strings
            cmd.Parameters.AddWithValue("folder", System.IO.Path.GetDirectoryName(abf.FilePath));
            cmd.Parameters.AddWithValue("filename", System.IO.Path.GetFileName(abf.FilePath));
            cmd.Parameters.AddWithValue("cjf_guid", GetCjfGuid(abf));
            cmd.Parameters.AddWithValue("date", abf.Header.uFileStartDate);
            cmd.Parameters.AddWithValue("protocol", System.IO.Path.GetFileNameWithoutExtension(abf.Header.sProtocolPath));
            cmd.Parameters.AddWithValue("duration_minutes", abfLengthMinutes);
            cmd.Parameters.AddWithValue("tags", abf.Tags.Count > 0 ? abf.Tags.ToString() : "");

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Return a GUID-like string from an ABF file.
        /// This string is expected to be unique for individual ABFs.
        /// </summary>
        public static string GetCjfGuid(AbfSharp.ABFFIO.ABF abf)
        {
            byte[] guidBytes = abf.Header.FileGUID.ToByteArray();
            UInt32 guidData1 = BitConverter.ToUInt32(guidBytes, 0);
            UInt16 guidData2 = BitConverter.ToUInt16(guidBytes, 4);
            UInt16 guidData3 = BitConverter.ToUInt16(guidBytes, 6);
            // NOTE: the last 8 bytes of the ABF's GUID are not used

            string guid = "";
            guid += guidData1.ToString("X");
            guid += guidData2.ToString("X");
            guid += guidData3.ToString("X");
            guid += abf.Header.lStopwatchTime.ToString("X");
            guid += abf.Header.uFileStartTimeMS.ToString("X");
            guid += abf.Header.ulFileCRC.ToString("X");
            guid += abf.Header.lActualAcqLength.ToString("X");
            guid += abf.Header.lTagSectionPtr.ToString("X");

            // convert a 4-byte float to a 4-byte unsigned integer and use its hex string
            byte[] scaleBytes = BitConverter.GetBytes(abf.Header.fInstrumentScaleFactor[0]);
            UInt32 scaleInt = BitConverter.ToUInt32(scaleBytes);
            guid += scaleInt.ToString("X");

            int guidLength = guid.Length;
            guid = guid.Insert(guidLength - guidLength / 4, "-");
            guid = guid.Insert(guidLength / 4, "-");
            guid = guid.Insert(guidLength / 2 + 1, "-");

            return guid;
        }
    }
}
