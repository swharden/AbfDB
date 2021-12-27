using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfDB.Tests
{
    internal class DatabaseBuilder
    {
        [Test]
        public void Test_AbfDB_CreateFromFolder()
        {
            string testDbFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".db");
            Console.WriteLine(testDbFile);
            AbfDB.DatabaseBuilder.Create(SampleData.ABF_FOLDER, testDbFile);
        }
    }
}
