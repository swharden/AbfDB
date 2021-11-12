using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbfWatcher.Models;

namespace AbfWatcher.Database
{
    internal class AbfDatabase
    {
        public readonly string DatabaseFolder;
        public int AbfCount => ABFs.Count;

        private readonly List<AbfRecord> ABFs = new();

        public AbfDatabase(string databaseFolder)
        {
            databaseFolder = Path.GetFullPath(databaseFolder);
            if (!Directory.Exists(databaseFolder))
                throw new DirectoryNotFoundException(databaseFolder);

            DatabaseFolder = databaseFolder;
        }

        public void Create(string path)
        {
            Debug.WriteLine($"Database CREATE: {path}");
        }

        public void Read(string path)
        {
            Debug.WriteLine($"Database READ: {path}");
        }

        public void Update(string path)
        {
            Debug.WriteLine($"Database UPDATE: {path}");
        }

        public void Delete(string path)
        {
            Debug.WriteLine($"Database DELETE: {path}");
        }
    }
}
