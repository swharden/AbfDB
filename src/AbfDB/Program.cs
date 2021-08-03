using System;
using System.Diagnostics;
using System.IO;

namespace AbfDB
{
    class Program
    {
        private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();
        private static int AbfsRead;

        static void Main(string[] args)
        {
            string scanFolder;
            string outputFolder;

            if (Debugger.IsAttached)
            {
                scanFolder = @"X:\Data\SD\OXT-Subiculum\Dose Experiments\10 nM 10 min exposure";
                outputFolder = "./";
            }
            else if (args.Length == 2)
            {
                scanFolder = Path.GetFullPath(args[0]);
                outputFolder = Path.GetFullPath(args[1]);
            }
            else
            {
                Console.WriteLine("ERROR: 2 arguments required (scan path and output file).");
                Console.WriteLine("Example: abfdb.exe \"c:\\scan\\folder\" \"c:\\output\\folder\"");
                return;
            }

            string abfLogFilePath = Path.Combine(outputFolder, "abfs.tsv");
            if (File.Exists(abfLogFilePath))
                File.Delete(abfLogFilePath);
            using var abfLog = File.AppendText(abfLogFilePath);

            string errorLogFilePath = Path.Combine(outputFolder, "log.txt");
            if (File.Exists(errorLogFilePath))
                File.Delete(errorLogFilePath);
            using var errorLog = File.AppendText(errorLogFilePath);

            errorLog.WriteLine($"[{DateTime.Now}] scanning {scanFolder}");
            abfLog.WriteLine(AbfInfo.GetTsvColumnNames());
            RecursivelyAddAbfs(scanFolder, abfLog, errorLog);
            Console.WriteLine($"Saved output in: {outputFolder}");
            errorLog.WriteLine($"\n[{DateTime.Now}] scanned {AbfsRead} ABFs in {Stopwatch.Elapsed}");
        }

        static void RecursivelyAddAbfs(string folderPath, StreamWriter abfLog, StreamWriter errorLog)
        {
            DirectoryInfo di = new(folderPath);

            foreach (FileInfo abfFile in di.GetFiles("*.abf"))
            {
                try
                {
                    var info = new AbfInfo(abfFile.FullName);
                    abfLog.WriteLine(info.GetTsvLine());
                    AbfsRead += 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EXCEPTION: {ex.Message}");
                    errorLog.WriteLine($"\nEXCEPTION: {abfFile.FullName}\n{ex}");
                }
            }

            Console.WriteLine($"[{Stopwatch.Elapsed}] [ABFs={AbfsRead:N0}] {folderPath}");

            foreach (DirectoryInfo subFolder in di.GetDirectories())
            {
                if (subFolder.Name.StartsWith("_"))
                    continue;
                RecursivelyAddAbfs(subFolder.FullName, abfLog, errorLog);
            }
        }
    }
}
