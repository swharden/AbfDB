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

        [Test]
        public void Test_Tsv_ToAndFrom()
        {
            foreach (string abfPath in SampleData.ABF_PATHS)
            {
                AbfRecord rec1 = AbfRecord.FromFile(abfPath);
                string tsv1 = rec1.ToTSV();

                AbfRecord rec2 = AbfRecord.FromTSV(tsv1);
                string tsv2 = rec2.ToTSV();

                Assert.AreEqual(tsv1, tsv2);
            }
        }
    }
}
