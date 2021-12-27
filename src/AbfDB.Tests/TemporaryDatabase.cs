using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB.Tests
{
    internal class TemporaryDatabase : IDisposable
    {
        public readonly string FilePath;
        private readonly AbfDB.AbfDatabase Database;

        internal TemporaryDatabase(string? path = null)
        {
            FilePath = path ?? Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".db");
            Console.WriteLine(FilePath);
            Database = new AbfDB.AbfDatabase(FilePath);
        }

        public void AddRandomEntry()
        {
            AbfRecord abfRecord = new()
            {
                Folder = @"C:\test",
                Filename = "test" + Path.GetRandomFileName() + ".abf",
                Guid = "test guid",
                Created = DateTime.Now,
                Protocol = "test protocol",
                LengthSec = 123.45,
                Comments = "test comments",
            };

            Database.Add(abfRecord);
        }

        public void Dispose()
        {
            Database.Dispose();
        }

        public int GetAbfCount()
        {
            Database.UpdateCount();
            return Database.Count;
        }
    }
}
