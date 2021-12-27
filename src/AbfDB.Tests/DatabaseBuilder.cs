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
        public void Test_Build_FromScratch()
        {
            string outFolder = Path.GetTempPath();
            Console.WriteLine(outFolder);
            var jb = new AbfDB.Jobs.CreateDbFromScratchJob(SampleData.ABF_FOLDER, outFolder);

            AbfDB.AbfDatabase db = new(jb.DatabaseFilePath);
            Assert.AreEqual(3, db.Count);
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

        [Test]
        public void Test_SqlDatabase_FromTsvFile()
        {
            string dbFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".db");
            Console.WriteLine(dbFilePath);

            _ = new AbfDB.Jobs.SqliteJob(SampleData.TSV_PATH, dbFilePath, limit: 100);

            var db = new AbfDatabase(dbFilePath);
            Assert.AreEqual(100, db.Count);
        }
    }
}
