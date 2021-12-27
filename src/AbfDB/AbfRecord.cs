using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB
{
    /// <summary>
    /// Represents a row in the database
    /// </summary>
    public record AbfRecord
    {
        public string Folder = string.Empty;
        public string Filename = string.Empty;
        public int SizeBytes = -1;
        public string Guid = string.Empty;
        public DateTime Recorded;
        public DateTime Noted;
        public string Protocol = string.Empty;
        public double LengthSec = -1;
        public string Comments = string.Empty;

        public static AbfRecord FromFile(string abfPath)
        {
            AbfRecord abfRecord = new();

            abfPath = Path.GetFullPath(abfPath);
            abfRecord.Folder = Path.GetDirectoryName(abfPath) ?? string.Empty;
            abfRecord.Filename = Path.GetFileName(abfPath);
            abfRecord.Noted = DateTime.Now;
            abfRecord.SizeBytes = (int)(new FileInfo(abfPath).Length);

            try
            {
                AbfSharp.ABFFIO.ABF abf = new(abfPath, preloadSweepData: false);
                abfRecord.Guid = AbfInfo.GetCjfGuid(abf);
                abfRecord.Recorded = AbfInfo.GetCreationDateTime(abf);
                abfRecord.Protocol = AbfInfo.GetProtocol(abf);
                abfRecord.LengthSec = AbfInfo.GetLengthSec(abf);
                abfRecord.Comments = AbfInfo.GetCommentSummary(abf);
            }
            catch (Exception ex)
            {
                if (AbfHasRsvFile(abfPath))
                {
                    System.Diagnostics.Debug.WriteLine($"INCOMPLETE ABF: {abfPath}");
                    abfRecord.Protocol = "INCOMPLETE";
                    abfRecord.Comments = "has RSV file";
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"ABF HEADER ERROR: {abfPath}");
                    abfRecord.Protocol = "EXCEPTION";
                    abfRecord.Comments = ex.Message;
                }
            }

            return abfRecord;
        }

        private static bool AbfHasRsvFile(string abfPath)
        {
            abfPath = Path.GetFullPath(abfPath);
            string abfID = Path.GetFileNameWithoutExtension(abfPath);
            string abfFolder = Path.GetDirectoryName(abfPath) ?? string.Empty;
            string rsvFilePath = Path.Combine(abfFolder, abfID + ".rsv");
            return File.Exists(rsvFilePath);
        }

        public string AsTSV()
        {
            string[] cells = new string[]
            {
                Folder,
                Filename,
                SizeBytes.ToString(),
                Guid,
                Recorded.ToString(),
                Noted.ToString(),
                Protocol,
                LengthSec.ToString(),
                Comments,
            };

            return string.Join("\t", cells);
        }

        public static string TsvColumns()
        {
            string[] names = new string[]
            {
                "Folder",
                "Filename",
                "SizeBytes",
                "Guid",
                "Recorded",
                "Noted",
                "Protocol",
                "LengthSec",
                "Comments",
            };

            return string.Join("\t", names);
        }
    }
}
