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
        public void Test_TsvDatabase_CreateFromFolder()
        {
            string testDbFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".tsv");
            Console.WriteLine(testDbFile);
            AbfDB.DatabaseBuilder.CreateTSV(SampleData.ABF_FOLDER, testDbFile);
        }
    }
}
