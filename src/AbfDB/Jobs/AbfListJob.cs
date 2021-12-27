using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB.Jobs
{
    public class AbfListJob : JobBase
    {
        /// <summary>
        /// Scan a folder tree, locate all ABF files, and store them in a text file (one per line)
        /// </summary>
        public AbfListJob(string searchPath)
        {
            FindABFs(new DirectoryInfo(searchPath));
            Completed("FOUND");
        }

        private void FindABFs(DirectoryInfo root)
        {
            string[] filePaths = Directory
                .GetFiles(root.FullName, "*.abf")
                .Where(x => x.EndsWith(".abf", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (string filePath in filePaths)
            {
                AbfPaths.Add(filePath);
                Console.WriteLine($"FOUND [{AbfCount}:N0] {filePath}");
            }

            foreach (DirectoryInfo dir in root.GetDirectories())
            {
                FindABFs(dir);
            }
        }
    }
}
