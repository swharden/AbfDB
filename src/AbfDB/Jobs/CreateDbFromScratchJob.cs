using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB.Jobs
{
    public class CreateDbFromScratchJob : JobBase
    {
        private readonly string Timestamp = DateTime.Now.Ticks.ToString();
        public readonly string ScanFolder;
        public readonly string OutFolder;
        private string BasePath => Path.Combine(OutFolder, Timestamp);
        public string TsvFilePath => BasePath + ".tsv";
        public string DatabaseFilePath => BasePath + ".db";

        /// <summary>
        /// Recursively scan a folder to build a new ABF database.
        /// </summary>
        public CreateDbFromScratchJob(string scanFolder, string outFolder)
        {
            ScanFolder = scanFolder;
            OutFolder = outFolder;

            Jobs.AbfListJob findJob = new(scanFolder);
            AbfPaths.AddRange(findJob.AbfPaths);
            Jobs.AbfReadJob readJob = new(AbfPaths, TsvFilePath);
            Jobs.SqliteJob databaseJob = new(TsvFilePath, DatabaseFilePath);

            Console.WriteLine(new String('-', 40));
            Console.WriteLine("SUMMARY:");
            findJob.Completed("FOUND");
            readJob.Completed("ANALYZED");
            databaseJob.Completed("INSERTED");
            Completed("EVERYTHING");
        }
    }
}
