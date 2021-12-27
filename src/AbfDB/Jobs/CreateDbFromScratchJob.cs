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

            var abfsFound = new Jobs.AbfListJob(scanFolder).AbfPaths;
            AbfPaths.AddRange(abfsFound);

            _ = new Jobs.AbfReadJob(AbfPaths, TsvFilePath);
            _ = new Jobs.SqliteJob(TsvFilePath, DatabaseFilePath);

            Completed("All tasks complete");
        }
    }
}
