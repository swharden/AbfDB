using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB
{
    /// <summary>
    /// This class contains logic for working with data in ABF headers
    /// </summary>
    public static class AbfFile
    {
        public static Database.AbfRecord GetRecord(string abfPath)
        {
            Database.AbfRecord abfRecord = new();

            abfPath = Path.GetFullPath(abfPath);
            abfRecord.Folder = Path.GetDirectoryName(abfPath) ?? string.Empty;
            abfRecord.Filename = Path.GetFileName(abfPath);
            abfRecord.Logged = DateTime.Now;
            abfRecord.SizeBytes = (int)(new FileInfo(abfPath).Length);

            try
            {
                AbfSharp.ABFFIO.ABF abf = new(abfPath, preloadSweepData: false);
                abfRecord.Guid = GetCjfGuid(abf);
                abfRecord.Recorded = GetCreationDateTime(abf);
                abfRecord.Protocol = GetProtocol(abf);
                abfRecord.LengthSec = GetLengthSec(abf);
                abfRecord.Comments = GetCommentSummary(abf);
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

        private static double GetLengthSec(AbfSharp.ABFFIO.ABF abf)
        {
            double sampleRate = 1e6 / abf.Header.fADCSequenceInterval / abf.Header.nADCNumChannels;
            return abf.Header.lActualAcqLength / abf.Header.nADCNumChannels / sampleRate;
        }

        /// <summary>
        /// Return a GUID-like string from an ABF file.
        /// This string is expected to be unique for individual ABFs.
        /// </summary>
        private static string GetCjfGuid(AbfSharp.ABFFIO.ABF abf)
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

        private static string GetProtocol(AbfSharp.ABFFIO.ABF abf)
        {
            return System.IO.Path.GetFileName(abf.Header.sProtocolPath.Trim());
        }

        /// <summary>
        /// Return all comments as a one-line comma-separated string
        /// </summary>
        private static string GetCommentSummary(AbfSharp.ABFFIO.ABF abf)
        {
            string[] tagSummaries = Enumerable.Range(0, abf.Tags.Count)
                .Select(x => $"{abf.Tags.Comments[x]} @ {abf.Tags.TimesMin[x]:0.00} min")
                .ToArray();

            return string.Join(", ", tagSummaries);
        }

        private static DateTime GetCreationDateTime(AbfSharp.ABFFIO.ABF abf)
        {
            int datecode = (int)abf.Header.uFileStartDate;

            int day = datecode % 100;
            datecode /= 100;

            int month = datecode % 100;
            datecode /= 100;

            int year = datecode;

            try
            {
                if (year < 1980 || year >= 2080)
                    throw new InvalidOperationException("unexpected creation date year in header");
                return new DateTime(year, month, day).AddMilliseconds(abf.Header.uFileStartTimeMS);
            }
            catch
            {
                return new DateTime(0);
            }
        }
    }
}
