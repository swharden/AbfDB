using System;
using System.IO;

namespace AbfDB
{
    /// <summary>
    /// This class holds methods that interact direclty with ABF files
    /// </summary>
    public static class AbfFile
    {
        /// <summary>
        /// Use AbfSharp to read the ABF header
        /// </summary>
        public static AbfInfo GetInfo(string abfFilePath)
        {
            AbfSharp.ABFFIO.ABF abf = new(abfFilePath, preloadSweepData: false);
            double sampleRate = 1e6 / abf.Header.fADCSequenceInterval / abf.Header.nADCNumChannels;

            return new AbfInfo()
            {
                FilePath = Path.GetFullPath(abfFilePath),
                SampleRate = sampleRate,
                LengthSec = abf.Header.lActualAcqLength / sampleRate,
                CjfGuid = GetCjfGuid(abf),
                Date = abf.Header.uFileStartDate,
                Protocol = Path.GetFileNameWithoutExtension(abf.Header.sProtocolPath),
                Tags = abf.Tags.Count > 0 ? abf.Tags.ToString() : "",
            };
        }

        /// <summary>
        /// Create a GUID-like string from an ABF file based on values from its header.
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
