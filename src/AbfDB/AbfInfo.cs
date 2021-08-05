using System;
using System.IO;
using System.Linq;

namespace AbfDB
{
    public class AbfInfo
    {
        public readonly string FilePath;
        public string Folder => Path.GetDirectoryName(FilePath);
        public string Filename => Path.GetFileName(FilePath);

        public readonly double SampleRate;
        public readonly double LengthSec;
        public readonly uint Date;
        public readonly string CjfGuid;
        public readonly string Protocol;
        public readonly string Tags;

        public AbfInfo(string abfFilePath)
        {
            AbfSharp.ABFFIO.ABF abf = new(abfFilePath, preloadSweepData: false);
            FilePath = Path.GetFullPath(abfFilePath);
            SampleRate = 1e6 / abf.Header.fADCSequenceInterval / abf.Header.nADCNumChannels;
            LengthSec = abf.Header.lActualAcqLength / SampleRate;
            CjfGuid = GetCjfGuid(abf);
            Date = abf.Header.uFileStartDate;
            Protocol = Path.GetFileNameWithoutExtension(abf.Header.sProtocolPath);
            Tags = abf.Tags.Count > 0 ? abf.Tags.ToString() : "";
        }

        public string GetFileMD5()
        {
            using var stream = File.OpenRead(FilePath);
            var md5 = System.Security.Cryptography.MD5.Create();
            return string.Join("", md5.ComputeHash(stream).Select(x => x.ToString("x2")));
        }

        public static string GetTsvColumnNames()
        {
            string[] parts =
            {
                "Folder",
                "Filename",
                "CjfGuid",
                "SampleRate (Hz)",
                "Length (sec)",
                "Date (int code)",
                "Protocol",
                "Tags"
            };

            return string.Join("\t", parts);
        }

        public string GetTsvLine()
        {
            string[] parts =
            {
                Folder.ToString(),
                Filename.ToString(),
                CjfGuid.ToString(),
                SampleRate.ToString(),
                LengthSec.ToString(),
                Date.ToString(),
                Protocol.ToString(),
                Tags.ToString()
            };

            return string.Join("\t", parts);
        }

        /// <summary>
        /// Return a GUID-like string from an ABF file based on values from its header.
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

            // NOTE: all bytes of floating point values are used to create their hex code
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
