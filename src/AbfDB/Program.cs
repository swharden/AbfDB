using System;
using System.Diagnostics;

namespace AbfDB
{
    class Program
    {

        static void Main(string[] args)
        {
            AbfScanner scanner;

            if (Debugger.IsAttached)
            {
                scanner = new AbfScanner(@"X:\Data\SD\OXT-Subiculum\Dose Experiments\10 nM 10 min exposure", "./");
            }
            else if (args.Length == 2)
            {
                scanner = new AbfScanner(args[0], args[1]);
            }
            else
            {
                Console.WriteLine("ERROR: 2 arguments required (scan path and output file).");
                Console.WriteLine("Example: abfdb.exe \"c:\\scan\\folder\" \"c:\\output\\folder\"");
                return;
            }

            scanner.Scan();
        }
    }
}
