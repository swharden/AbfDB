using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB.Jobs
{
    public class AbfReadJob : JobBase
    {
        /// <summary>
        /// Scan a folder tree and generate a new database from scratch.
        /// Recording data in a TSV file greatly improves speed over logging in a SQL database.
        /// The file buffer grows in memory and is only occasionally flushed/written.
        /// In contrast the SQL database chuggs the hard disk every time a record is inserted.
        /// </summary>
        public AbfReadJob(List<string> abfPaths, string tsvPath)
        {
            if (File.Exists(tsvPath))
                throw new InvalidOperationException($"ERROR - file already exists: {tsvPath}");

            AbfPaths.AddRange(abfPaths);

            // TODO: move this logic here
            using TsvBuilder database = new(tsvPath);
            for (int i = 0; i < AbfCount; i++)
            {
                string abfPath = AbfPaths[i];
                Console.WriteLine($"READING [{i + 1:N0} of {AbfCount:N0}] {abfPath}");
                database.Add(abfPath);
            }

            Completed("READ");
        }
    }
}
