using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AbfDB
{
    /// <summary>
    /// This class recursively scans a folder looking for ABFs, 
    /// reads their headers with AbfSharp, and logs the output to a TSV file.
    /// </summary>
    public class AbfScanner
    {
        private readonly string ScanFolder;
        private readonly string OutFolder;
        private string LogFilePath => Path.Combine(OutFolder, "log.txt");
        private string TsvFilePath => Path.Combine(OutFolder, "abfs.tsv");

        private readonly Stopwatch Stopwatch = new();
        private int AbfsRead;

        private StreamWriter TsvWriter;
        private StreamWriter LogWriter;

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
            DeleteExistingFile(TsvFilePath);
            DeleteExistingFile(LogFilePath);
        }

        private static void DeleteExistingFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        private void OpenLogFiles()
        {
            TsvWriter = File.AppendText(TsvFilePath);
            LogWriter = File.AppendText(LogFilePath);

            TsvWriter.WriteLine(string.Join("\t", TsvFile.ColumnNames));
            LogWriter.WriteLine($"[{DateTime.Now}] scanning {ScanFolder}");
        }

        private void CloseLogFiles()
        {
            LogWriter.WriteLine($"\n[{DateTime.Now}] scanned {AbfsRead} ABFs in {Stopwatch.Elapsed}");
            Console.WriteLine($"Output saved in: {OutFolder}");

            LogWriter.Close();
            TsvWriter.Close();
        }

        private void RecursivelyAddAbfs(string folderPath)
        {
            DirectoryInfo di = new(folderPath);

            string[] abfFilePaths = di.GetFiles("*.abf")
                .Select(x => x.FullName)
                .ToArray();

            string[] subFolderPaths = di.GetDirectories()
                .Where(x => !x.Name.StartsWith("_"))
                .Select(x => x.FullName)
                .ToArray();

            foreach (string abfFilePath in abfFilePaths)
                AddAbf(abfFilePath);

            Console.WriteLine($"[{Stopwatch.Elapsed}] [ABFs={AbfsRead:N0}] {di.FullName}");

            foreach (string subFolderPath in subFolderPaths)
                RecursivelyAddAbfs(subFolderPath);
        }

        private void AddAbf(string abfFilePath)
        {
            try
            {
                AbfInfo info = AbfFile.GetInfo(abfFilePath);
                TsvWriter.WriteLine(TsvFile.MakeLine(info));
                AbfsRead += 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
                LogWriter.WriteLine($"\nEXCEPTION: {abfFilePath}\n{ex}");
            }
        }
    }
}
