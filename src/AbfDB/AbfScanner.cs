using System;
using System.Diagnostics;
using System.IO;

namespace AbfDB
{
    public class AbfScanner
    {
        public readonly string ScanFolder;
        public readonly string OutFolder;
        public string LogFilePath => Path.Combine(OutFolder, "log.txt");
        public string TsvFilePath => Path.Combine(OutFolder, "abfs.tsv");

        private readonly Stopwatch Stopwatch = new();
        private int AbfsRead;

        StreamWriter TsvFile;
        StreamWriter LogFile;

        public AbfScanner(string scanFolder, string outFolder)
        {
            if (!Directory.Exists(scanFolder))
                throw new ArgumentException($"scan folder does not exist: {scanFolder}");

            if (!Directory.Exists(outFolder))
                throw new ArgumentException($"output folder does not exist: {outFolder}");

            ScanFolder = Path.GetFullPath(scanFolder);
            OutFolder = Path.GetFullPath(outFolder);
        }

        public void Scan()
        {
            if (AbfsRead > 0)
                throw new InvalidOperationException("scan must only be performed once");

            Stopwatch.Restart();
            DeleteOldLogFiles();
            OpenLogFiles();
            RecursivelyAddAbfs(ScanFolder);
            CloseLogFiles();
        }

        private void DeleteOldLogFiles()
        {
            if (File.Exists(LogFilePath))
                File.Delete(LogFilePath);
            if (File.Exists(TsvFilePath))
                File.Delete(TsvFilePath);
        }

        private void OpenLogFiles()
        {
            TsvFile = File.AppendText(TsvFilePath);
            LogFile = File.AppendText(LogFilePath);

            TsvFile.WriteLine(AbfInfo.GetTsvColumnNames());
            LogFile.WriteLine($"[{DateTime.Now}] scanning {ScanFolder}");
        }

        public void CloseLogFiles()
        {
            LogFile.WriteLine($"\n[{DateTime.Now}] scanned {AbfsRead} ABFs in {Stopwatch.Elapsed}");
            LogFile.Close();
            TsvFile.Close();
            Console.WriteLine($"Output saved in: {OutFolder}");
        }

        private void RecursivelyAddAbfs(string folderPath)
        {
            DirectoryInfo di = new(folderPath);

            foreach (FileInfo abfFileInfo in di.GetFiles("*.abf"))
            {
                try
                {
                    AbfInfo info = new(abfFileInfo.FullName);
                    TsvFile.WriteLine(info.GetTsvLine());
                    AbfsRead += 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EXCEPTION: {ex.Message}");
                    LogFile.WriteLine($"\nEXCEPTION: {abfFileInfo.FullName}\n{ex}");
                }
            }

            Console.WriteLine($"[{Stopwatch.Elapsed}] [ABFs={AbfsRead:N0}] {folderPath}");

            foreach (DirectoryInfo subFolder in di.GetDirectories())
            {
                if (subFolder.Name.StartsWith("_"))
                    continue;
                RecursivelyAddAbfs(subFolder.FullName);
            }
        }
    }
}
