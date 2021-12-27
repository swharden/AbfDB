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
            Database.Add(
                folder: "test folder",
                filename: "test" + Path.GetRandomFileName() + ".abf",
                guid: "test guid",
                created: DateTime.Now,
                protocol: "test protocol",
                lengthSec: 123.45,
                comments: "test comments");
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
