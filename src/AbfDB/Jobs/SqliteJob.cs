using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB.Jobs
{
    public class SqliteJob : JobBase
    {
        /// <summary>
        /// Create (or extend) a SQL database given a TSV file of ABF file info.
        /// </summary>
        public SqliteJob(string tsvPath, string dbPath, int limit = int.MaxValue)
        {
            if (File.Exists(dbPath))
                throw new InvalidOperationException($"ERROR - file already exists: {dbPath}");

            AbfRecord[] abfs = File.ReadLines(tsvPath)
                .Skip(1)
                .Where(x => x.Length > 10)
                .Select(x => AbfRecord.FromTSV(x))
                .Take(limit)
                .ToArray();

            AbfPaths.AddRange(abfs.Select(x => x.FullPath));

            AbfDatabase database = new(dbPath);
            for (int i = 0; i < abfs.Length; i++)
            {
                Console.WriteLine($"INSERTING [{i + 1:N0} of {AbfCount:N0}] {abfs[i].FullPath}");
                database.Add(abfs[i]);
            }

            Completed("INSERTED");
        }
    }
}
