using CommandLine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace AbfDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new string[] { @"C:/", "test.tsv" };

            string basePath = args[0];
            string tsvPath = args[1];

            var sw = Stopwatch.StartNew();
            Console.WriteLine("Reading indexed filesystem for ABF and RSV files...");
            Dictionary<string, IndexedAbf> abfs = WinSearch.GetIndexedAbfs(basePath);
            Console.WriteLine($"Located {abfs.Count:N0} ABFs in {sw.Elapsed.TotalSeconds:0.00} sec");

            WinSearchTSV.Save(abfs, tsvPath);
        }
    }
}