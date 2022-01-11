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
            BuildSqlDatabase("test.db", @"L:\abfdb\indexedAbfs.tsv");
        }

        public static void BuildTsvDatabase(string basePath, string tsvPath)
        {
            var sw = Stopwatch.StartNew();
            Console.WriteLine("Reading indexed filesystem for ABF and RSV files...");
            Dictionary<string, IndexedAbf> abfs = WinSearch.GetIndexedAbfs(basePath);
            Console.WriteLine($"Located {abfs.Count:N0} ABFs in {sw.Elapsed.TotalSeconds:0.00} sec");
            WinSearchTSV.Save(abfs, tsvPath);
        }

        public static void BuildSqlDatabase(string dbPath, string tsvPath)
        {
            Dictionary<string, IndexedAbf> abfs = WinSearchTSV.Load(tsvPath);
            AbfDB.AbfDatabase db = new(dbPath);
        }
    }
}