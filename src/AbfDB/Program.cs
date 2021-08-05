using System;
using System.Diagnostics;
using System.IO;

namespace AbfDB
{
    class Program
    {

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

            var scanner = new AbfScanner(scanFolder, outputFolder);
            scanner.Scan();
        }
    }
}
